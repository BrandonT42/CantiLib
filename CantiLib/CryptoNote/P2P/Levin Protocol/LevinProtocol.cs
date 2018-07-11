using Canti.Data;
using Canti.Utilities;
using System;
using System.Collections.Generic;

namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        // Server connection
        private Server Server;

        // Logger
        private Logger Logger;

        // Peer read status (0 = head, 1 = body)
        private Dictionary<PeerConnection, LevinPeer> Peers = new Dictionary<PeerConnection, LevinPeer>();

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Set connection
            Server = Connection;

            // Set logger
            Logger = Connection.Logger;

            // Bind event handlers
            Server.OnDataReceived += OnDataReceived;
            Server.OnPeerConnected += OnPeerConnected;
            Server.OnPeerDisconnected += OnPeerDisconnected;
        }

        // Data received
        private void OnDataReceived(object sender, EventArgs e)
        {
            // Get packet data
            Packet Packet = (Packet)sender;
            LevinPeer Peer = Peers[Packet.Peer];

            // Read header
            if (Peer.ReadStatus == PacketReadStatus.Head)
            {
                // Decode header
                try { Peer.Header = BucketHead2.Deserialize(Packet.Data); }
                catch
                {
                    Logger?.Log(Level.DEBUG, "Could not deserialize incoming packet header, incorrect format");
                    return;
                }

                // Set peer data
                Peer.Data = Packet.Data;

                // Debug
                Logger?.Log(Level.DEBUG, "Received header:");
                Logger?.Log(Level.DEBUG, " - Signature: {0}", Peers[Packet.Peer].Header.Signature);
                Logger?.Log(Level.DEBUG, " - Payload Size: {0}", Peers[Packet.Peer].Header.PayloadSize);
                Logger?.Log(Level.DEBUG, " - Response Required: {0}", Peers[Packet.Peer].Header.ResponseRequired);
                Logger?.Log(Level.DEBUG, " - Command Code: {0}", Peers[Packet.Peer].Header.CommandCode);
                Logger?.Log(Level.DEBUG, " - Return Code: {0}", Peers[Packet.Peer].Header.ReturnCode);
                Logger?.Log(Level.DEBUG, " - Flags: {0}", Peers[Packet.Peer].Header.Flags);
                Logger?.Log(Level.DEBUG, " - Protocol Version: {0}", Peers[Packet.Peer].Header.ProtocolVersion);

                // Check that signature matches
                if (Peer.Header.Signature != GlobalsConfig.LEVIN_SIGNATURE)
                {
                    Logger?.Log(Level.DEBUG, "Incoming packet signature mismatch, expected {0}, received {1}", GlobalsConfig.LEVIN_SIGNATURE, Peers[Packet.Peer].Header.Signature);
                    return;
                }

                // Check packet size
                if (Peer.Header.PayloadSize > GlobalsConfig.LEVIN_MAX_PACKET_SIZE)
                {
                    Logger?.Log(Level.DEBUG, "Incoming packet size is too big, max size is {0}, received {1}", GlobalsConfig.LEVIN_MAX_PACKET_SIZE, Packet.Data.Length);
                    return;
                }

                // Set new read status
                if (Peer.Header.PayloadSize > 0) Peers[Packet.Peer].ReadStatus = PacketReadStatus.Body;
            }

            // Add data to peer buffer if reading message body
            else
            {
                // Add bytes to peer buffer
                byte[] NewData = new byte[Peer.Data.Length + Packet.Data.Length];
                Buffer.BlockCopy(Peer.Data, 0, NewData, 0, Peer.Data.Length);
                Buffer.BlockCopy(Packet.Data, 0, NewData, Peer.Data.Length, Packet.Data.Length);
                Peer.Data = NewData;
            }

            // Check if data size matches payload size and that a header has been decoded
            if (Peer.ReadStatus == PacketReadStatus.Body && (ulong)Peer.Data.Length >= Peer.Header.PayloadSize)
            {
                // Get header
                BucketHead2 Header = Peer.Header;

                // Decode command
                Command Command = new Command
                {
                    CommandCode = Header.CommandCode,
                    IsNotification = !Header.ResponseRequired,
                    IsResponse = (Header.Flags & LEVIN_PACKET_RESPONSE) == LEVIN_PACKET_RESPONSE,
                    Data = Encoding.SplitByteArray(Peers[Packet.Peer].Data, 33, (int)Peers[Packet.Peer].Header.PayloadSize)
                };

                // Debug
                Logger?.Log(Level.DEBUG, "Received command:");
                Logger?.Log(Level.DEBUG, " - Command Code: {0}", Command.CommandCode);
                Logger?.Log(Level.DEBUG, " - Is Notification: {0}", Command.IsNotification);
                Logger?.Log(Level.DEBUG, " - Is Response: {0}", Command.IsResponse);
                Logger?.Log(Level.DEBUG, " - Data: {0}", Encoding.ByteArrayToHexString(Command.Data));

                // Deserialize response
                Commands.CommandHandshake.Response Response = new Commands.CommandHandshake.Response();
                Response = Response.Deserialize(Command.Data);

                // Send response
                // TODO

                // Set new read status and clear previous request
                Peer.ReadStatus = PacketReadStatus.Head;
                Peer.Header = default(BucketHead2);
                Peer.Data = new byte[0];
            }
        }

        // Peer connected
        private void OnPeerConnected(object sender, EventArgs e)
        {
            // Get peer connection
            PeerConnection Peer = (PeerConnection)sender;

            // Add peer to peer list
            Peers.Add(Peer, new LevinPeer());
        }

        // Peer disconnected
        private void OnPeerDisconnected(object sender, EventArgs e)
        {
            // Get peer connections
            PeerConnection Peer = (PeerConnection)sender;

            // Remove peer from peer list
            Peers.Remove(Peer);
        }

        // Notifies a peer with a command, no response expected
        internal void Notify(PeerConnection Peer, int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired = false,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_REQUEST
            };

            // Send header packet
            if (Server.SendMessage(Peer, Header.Serialize()))
            {
                // Send body packet
                Server.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected
        internal void NotifyAll(int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired =  false,
                PayloadSize =       (ulong)Data.Length,
                CommandCode =       (uint)CommandCode,
                ProtocolVersion =   LEVIN_PROTOCOL_VER_1,
                Flags =             LEVIN_PACKET_REQUEST
            };

            // Send header packet
            Server.Broadcast(Header.Serialize());
            
            // Send body packet
            Server.Broadcast(Data);
        }

        // Notifies a peer with a command, no response expected
        internal void Reply(PeerConnection Peer, int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired = false,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_RESPONSE
            };

            // Send header packet
            if (Server.SendMessage(Peer, Header.Serialize()))
            {
                // Send body packet
                Server.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected (DEBUG PURPOSES)
        internal void SendMessageAll(int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = GlobalsConfig.LEVIN_SIGNATURE,
                ResponseRequired = true,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_REQUEST
            };

            // Debug
            Logger?.Log(Level.DEBUG, "Sending header:");
            Logger?.Log(Level.DEBUG, " - Signature: {0}", Header.Signature);
            Logger?.Log(Level.DEBUG, " - Payload Size: {0}", Header.PayloadSize);
            Logger?.Log(Level.DEBUG, " - Response Required: {0}", Header.ResponseRequired);
            Logger?.Log(Level.DEBUG, " - Command Code: {0}", Header.CommandCode);
            Logger?.Log(Level.DEBUG, " - Return Code: {0}", Header.ReturnCode);
            Logger?.Log(Level.DEBUG, " - Flags: {0}", Header.Flags);
            Logger?.Log(Level.DEBUG, " - Protocol Version: {0}", Header.ProtocolVersion);

            Command Command = new Command
            {
                CommandCode = Header.CommandCode,
                IsNotification = !Header.ResponseRequired,
                IsResponse = (Header.Flags & LEVIN_PACKET_RESPONSE) == LEVIN_PACKET_RESPONSE,
                Data = Data
            };

            // Debug
            Logger?.Log(Level.DEBUG, "Sending command:");
            Logger?.Log(Level.DEBUG, " - Command Code: {0}", Command.CommandCode);
            Logger?.Log(Level.DEBUG, " - Is Notification: {0}", Command.IsNotification);
            Logger?.Log(Level.DEBUG, " - Is Response: {0}", Command.IsResponse);
            Logger?.Log(Level.DEBUG, " - Data: {0}", Encoding.ByteArrayToHexString(Command.Data));

            // Send header packet
            //Connection.Broadcast(Header.Serialize());

            // Send body packet
            //Connection.Broadcast(Data);
            Server.Broadcast(Encoding.AppendToByteArray(Data, Header.Serialize()));
        }

        // Encodes a command and returns the raw bytes
        internal byte[] Encode(ICommandRequestBase Data)
        {
            // Return the serialized byte array
            return Data.Serialize();
        }

        // Decodes a command to a specified type
        internal T Decode<T>(byte[] Data)
        {
            // Verify type
            if (!typeof(ICommandResponseBase<T>).IsAssignableFrom(typeof(T)))
            {
                Logger?.Log(Level.DEBUG, "Failed to decode command response - incorrect type: {0}, expected type ICommandResponseBase<T>", typeof(T).FullName);
                return default(T);
            }

            // Create a default object of the deserialized type
            ICommandResponseBase<T> O = default(T) as ICommandResponseBase<T>;

            // Return the deserialized object
            return O.Deserialize(Data);
        }
    }
}
