//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;

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
            P2pServer.OnPeerConnected += OnPeerConnected;
            P2pServer.OnPeerDisconnected += OnPeerDisconnected;
            P2pServer.OnDataReceived += OnDataReceived;
            P2pServer.OnDataSent += OnDataSent;
        }

        #region Event Callbacks

        // This is invoked when our underlying P2P server is started successfully
        public void OnStart(object sender, EventArgs e) { }

        // This is invoked when our underlying P2P server is stopped
        public void OnStop(object sender, EventArgs e) { }

        // This is invoked when a new P2P peer connection is formed
        public void OnPeerConnected(object sender, EventArgs e)
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
                Logger.Debug($"[{P2pPeer.Address} IN] CONNECTION FORMED");
            }
        }

        // This is invoked when a connected P2P peer is disconnected
        public void OnPeerDisconnected(object sender, EventArgs e)
        {
            // Get peer data
            var Peer = (P2pPeer)sender;

            // Remove peer from peer list
            RemovePeer(Peer);
            Logger.Debug($"[{Peer.Address} OUT] PEER DISCONNECTED");
        }

        // This is invoked when our underlying P2P server receives data from a peer
        public void OnDataReceived(object sender, EventArgs e)
        {
            // Get the incoming data
            var Data = ((P2pPeer Peer, byte[] Buffer))sender;

            // Add data to our packet buffer
            GetPeer(Data.Peer).AddData(Data.Buffer);
        }

        // This is invoked when our underlying P2P server sends data to a peer
        public void OnDataSent(object sender, EventArgs e) { }

        // This is called when we receive a valid packet
        internal void OnPacketReceived(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger.Debug($"[{Peer.Address} IN] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
            //PrintPacketBody(Packet.Body);
        }

        // This is called when we send a packet
        internal void OnPacketSent(Peer Peer, Packet Packet)
        {
            // Log debug message
            Logger.Debug($"[{Peer.Address} OUT] {Packet.Type} {Packet.Flag} (VALIDATED: {Peer.Validated}, " +
                $"RESPONSE REQUIRED: {Packet.Header.ResponseRequired})");
        }

        #endregion

        #endregion

        // TODO - DEBUG CODE
        static void PrintPacketBody(dynamic Body, int Depth = 1)
        {
            int Index = 0;
            foreach (KeyValuePair<string, dynamic> Entry in Body)
            {
                Type ValueType = Entry.Value.GetType();
                if (ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                    PrintPacketBody(Entry.Value, Depth + 1);
                }
                else if (Entry.Key == "block_ids")
                {
                    string Output = $"[{Index}] {Entry.Key}:";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                    for (var i = 0; i < Entry.Value.Length; i += 64)
                    {
                        string tmp = $"[{i / 64}] {Entry.Value.Substring(i, 64)}";
                        tmp = tmp.PadLeft(tmp.Length + ((Depth + 1) * 2));
                        Logger.Debug(tmp);
                    }
                }
                else
                {
                    string Output = $"[{Index}] {Entry.Key}: {Entry.Value}";
                    Output = Output.PadLeft(Output.Length + (Depth * 2));
                    Logger.Debug(Output);
                }
                Index++;
            }
        }
    }
}
