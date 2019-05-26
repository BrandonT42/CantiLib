//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node : INode
    {
        #region Methods

        // Assigns our callbacks to our underlying P2P server's corresponding event handlers
        private void AssignCallbacks()
        {
            P2pServer.OnStart += OnStart;
            P2pServer.OnStop += OnStop;
            P2pServer.OnPeerConnected += OnP2pPeerConnected;
            P2pServer.OnPeerDisconnected += OnP2pPeerDisconnected;
            P2pServer.OnDataReceived += OnDataReceived;
            P2pServer.OnDataSent += OnDataSent;
        }

        #endregion

        #region Events Callbacks

        #region Public

        /// <summary>
        /// Invoked when the underlying P2P server is started successfully
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public void OnStart(object sender, EventArgs e) { }

        /// <summary>
        /// Invoked when the underlying P2P server is stopped
        /// </summary>
        /// <param name="sender">A reference to the P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public void OnStop(object sender, EventArgs e) { }

        /// <summary>
        /// Invoked when a new peer connection is detected by the underlying P2P server
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public void OnP2pPeerConnected(object sender, EventArgs e)
        {
            // Get peer data
            var P2pPeer = (P2pPeer)sender;

            // Check if this peer is already in our peer list
            if (PeerList.Count(x => x.P2pPeer == P2pPeer) == 0)
            {
                // Add this peer to our peer list
                var Peer = AddPeer(P2pPeer);

                // Handshake with this peer
                if (!P2pPeer.IsIncoming)
                {
                    Handshake(Peer);
                }
            }
        }

        /// <summary>
        /// Invoked when a peer's polling attempt comes back unsuccessful, indicating a disconnection
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public void OnP2pPeerDisconnected(object sender, EventArgs e)
        {
            // Get peer data
            var Peer = (P2pPeer)sender;

            // Remove peer from peer list
            RemovePeer(Peer);
        }

        /// <summary>
        /// Invoked when a connected peer sends the underlying P2P server data
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data received</param>
        /// <param name="e">Always empty</param>
        public void OnDataReceived(object sender, EventArgs e)
        {
            // Get the incoming data
            var Data = ((P2pPeer Peer, byte[] Buffer))sender;

            // Add data to our packet buffer
            GetPeer(Data.Peer).AddData(Data.Buffer);
        }

        /// <summary>
        /// Invoked when the underlying P2P server sends data to a connected peer
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data sent</param>
        /// <param name="e">Always empty</param>
        public void OnDataSent(object sender, EventArgs e) { }

        #endregion

        #region Internal

        // This is called when we receive a valid packet
        internal void OnPacketReceived(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger?.Debug($"[{Peer.Address} IN] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
            //PrintPacketBody(Packet.Body);

            // Wrap this in a try-catch, in case we received an invalid packet
            try
            {
                // Handle this packet
                switch (Packet.Type)
                {
                    case PacketType.HANDSHAKE:
                        HandleHandshake(Peer, Packet);
                        break;

                    case PacketType.TIMED_SYNC:
                        HandleTimedSync(Peer, Packet);
                        break;
                }

                // If we reach this far, the packet was valid, set peer
                Peer.LastSeen = GetTimestamp();
            }

            // We use this exception type when a packet contains invalid data
            catch (InvalidOperationException e)
            {
                Logger.Warning(e.Message);
                Peer.Dispose();
            }

            // If this catch fires, the packet was improperly formated
            catch
            {
                Logger.Warning($"Invalid {Packet.Type} packet received from {Peer.Address}, killing connection...");
                Peer.Dispose();
            }
        }

        // This is called when we send a packet
        internal void OnPacketSent(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger?.Debug($"[{Peer.Address} OUT] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
        }

        // This is called when a handshake is accepted
        internal void OnPeerConnected(Peer Peer)
        {
            // Log connection message
            Logger?.Debug($"[{Peer.Address} IN] CONNECTION FORMED");
        }

        // This is called when a peer is removed from our peer list
        internal void OnPeerDisconnected(Peer Peer)
        {
            // Log disconnection message
            Logger?.Debug($"[{Peer.Address} OUT] PEER DISCONNECTED");
        }

        #endregion

        #endregion

        // TODO - DEBUG CODE
        private void PrintPacketBody(dynamic Body, int Depth = 1)
        {
            int Index = 0;
            foreach (KeyValuePair<string, dynamic> Entry in Body)
            {
                Type ValueType = Entry.Value.GetType();
                if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger?.Debug(Output);
                    PrintPacketBody(Entry.Value, Depth + 1);
                }
                else if (Entry.Key == "block_ids")
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger?.Debug(Output);
                    for (var i = 0; i < Entry.Value.Length; i += 64)
                    {
                        string tmp = $"[{i / 64}] {Entry.Value.Substring(i, 64)}";
                        tmp = tmp.PadLeft(tmp.Length + ((Depth + 1) * 2));
                        Logger?.Debug(tmp);
                    }
                }
                else
                {
                    string Output = $"[{Index}] {Entry.Key}: {Entry.Value}";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger?.Debug(Output);
                }
                Index++;
            }
        }
    }
}
