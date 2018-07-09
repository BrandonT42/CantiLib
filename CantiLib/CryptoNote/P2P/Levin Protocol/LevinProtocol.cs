using Canti.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        // Server connection
        private Server Connection;

        // Peer read status (0 = head, 1 = body)
        private Dictionary<PeerConnection, LevinPeer> Peers = new Dictionary<PeerConnection, LevinPeer>();

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Set connection
            this.Connection = Connection;

            // Bind event handlers
            this.Connection.OnDataReceived += OnDataReceived;
            this.Connection.OnPeerConnected += OnPeerConnected;
            this.Connection.OnPeerDisconnected += OnPeerDisconnected;
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
                Peer.Header = BucketHead2.Deserialize(Packet.Data);

                // Set peer data
                Peer.Data = Packet.Data;

                // Debug
                Console.WriteLine("Received header:");
                Console.WriteLine(" - Signature: {0}", Peers[Packet.Peer].Header.Signature);
                Console.WriteLine(" - Payload Size: {0}", Peers[Packet.Peer].Header.PayloadSize);
                Console.WriteLine(" - Response Required: {0}", Peers[Packet.Peer].Header.ResponseRequired);
                Console.WriteLine(" - Command Code: {0}", Peers[Packet.Peer].Header.CommandCode);
                Console.WriteLine(" - Return Code: {0}", Peers[Packet.Peer].Header.ReturnCode);
                Console.WriteLine(" - Flags: {0}", Peers[Packet.Peer].Header.Flags);
                Console.WriteLine(" - Protocol Version: {0}", Peers[Packet.Peer].Header.ProtocolVersion);

                // Check that signature matches
                if (Peer.Header.Signature != LEVIN_SIGNATURE)
                {
                    Console.WriteLine("Signature mismatch, got {0}, expected {1}", Peers[Packet.Peer].Header.Signature, LEVIN_SIGNATURE);
                    return; // TODO - throw error
                }

                // Check packet size
                if (Peer.Header.PayloadSize > LEVIN_DEFAULT_MAX_PACKET_SIZE)
                {
                    Console.WriteLine("Packet size too big");
                    return; // TODO - throw error
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

            Console.WriteLine("Need {0} data, have {1}", Peer.Header.PayloadSize, Peers[Packet.Peer].Data.Length);

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
                Console.WriteLine("Received command:");
                Console.WriteLine(" - Command Code: {0}", Command.CommandCode);
                Console.WriteLine(" - Is Notification: {0}", Command.IsNotification);
                Console.WriteLine(" - Is Response: {0}", Command.IsResponse);
                Console.WriteLine(" - Data: {0}", Encoding.ByteArrayToHexString(Command.Data));
                //byte[] Decompressed = Encoding.DecompressByteArray(Command.Data);
                //Console.WriteLine(" - Decompressed data: {0}", Decompressed);
                Console.WriteLine("Attempting to parse to request...");
                //Commands.CommandHandshake.Response Response = new Commands.CommandHandshake.Response();
                //Response = Response.Deserialize(Command.Data);

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
                Signature = LEVIN_SIGNATURE,
                ResponseRequired = false,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_REQUEST
            };

            // Send header packet
            if (Connection.SendMessage(Peer, Header.Serialize()))
            {
                // Send body packet
                Connection.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected
        internal void NotifyAll(int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature =         LEVIN_SIGNATURE,
                ResponseRequired =  false,
                PayloadSize =       (ulong)Data.Length,
                CommandCode =       (uint)CommandCode,
                ProtocolVersion =   LEVIN_PROTOCOL_VER_1,
                Flags =             LEVIN_PACKET_REQUEST
            };

            // Send header packet
            Connection.Broadcast(Header.Serialize());
            
            // Send body packet
            Connection.Broadcast(Data);
        }

        // Notifies a peer with a command, no response expected
        internal void Reply(PeerConnection Peer, int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = LEVIN_SIGNATURE,
                ResponseRequired = false,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_RESPONSE
            };

            // Send header packet
            if (Connection.SendMessage(Peer, Header.Serialize()))
            {
                // Send body packet
                Connection.SendMessage(Peer, Data);
            }
        }

        // Notifies all peers with a command, no response expected
        internal void SendMessageAll(int CommandCode, byte[] Data)
        {
            // Form message header
            BucketHead2 Header = new BucketHead2
            {
                Signature = LEVIN_SIGNATURE,
                ResponseRequired = true,
                PayloadSize = (ulong)Data.Length,
                CommandCode = (uint)CommandCode,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_REQUEST
            };

            // Debug
            Console.WriteLine("Sending header:");
            Console.WriteLine(" - Signature: {0}", Header.Signature);
            Console.WriteLine(" - Payload Size: {0}", Header.PayloadSize);
            Console.WriteLine(" - Response Required: {0}", Header.ResponseRequired);
            Console.WriteLine(" - Command Code: {0}", Header.CommandCode);
            Console.WriteLine(" - Return Code: {0}", Header.ReturnCode);
            Console.WriteLine(" - Flags: {0}", Header.Flags);
            Console.WriteLine(" - Protocol Version: {0}", Header.ProtocolVersion);

            Command Command = new Command
            {
                CommandCode = Header.CommandCode,
                IsNotification = !Header.ResponseRequired,
                IsResponse = (Header.Flags & LEVIN_PACKET_RESPONSE) == LEVIN_PACKET_RESPONSE,
                Data = Data
            };

            // Debug
            Console.WriteLine("Sending command:");
            Console.WriteLine(" - Command Code: {0}", Command.CommandCode);
            Console.WriteLine(" - Is Notification: {0}", Command.IsNotification);
            Console.WriteLine(" - Is Response: {0}", Command.IsResponse);
            Console.WriteLine(" - Data: {0}", Encoding.ByteArrayToHexString(Command.Data));

            // Send header packet
            Connection.Broadcast(Header.Serialize());

            // Send body packet
            Connection.Broadcast(Data);
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
                Connection.OnError?.Invoke("Failed to decode command response - incorrect type: " + typeof(T).FullName + ", expected type ICommandResponseBase<T>", EventArgs.Empty);
                return default(T);
            }

            // Create a default object of the deserialized type
            ICommandResponseBase<T> O = default(T) as ICommandResponseBase<T>;

            // Return the deserialized object
            return O.Deserialize(Data);
        }
    }
}
