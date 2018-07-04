using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Canti.Data;

// TODO:
// - Handshake
// - Figure out that ReadStrict shit to see what it's wanting
namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        // Variables
        private Server Connection;

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Link server
            this.Connection = Connection;

            // Add an event handler to the OnCommandReceived event
            this.Connection.OnCommandReceived += delegate (object sender, EventArgs e)
            {
                // Get command data
                Command Command = (Command)sender;

                // Command is a handshake
                if (Command.CommandCode == Commands.Handshake.Id) HandleHandshakeRequest(Command);

                // Command is a ping
                else if (Command.CommandCode == Commands.Ping.Id)
                {
                    /*Commands.Ping.Response resp = new Commands.Ping.Response();
                    //    resp.PING_OK_RESPONSE_STATUS_TEXT, m_node.getPeerId() };
                    Context.SendMessage(MakeReply(Commands.Ping.Id, LevinProtocol.Encode(resp), LevinProtocol.LEVIN_PROTOCOL_RETCODE_SUCCESS));
                    return false;*/
                    SendReply(Command.Source.GetStream(), (uint)Commands.Ping.Id, Encode("response"), LEVIN_PROTOCOL_RETCODE_SUCCESS);
                }

                // Unknown command
                else Connection.OnError?.Invoke("Unexpected command: " + Command.CommandCode, EventArgs.Empty);
            };

            // Add an event handler to the OnDataReceived event
            this.Connection.OnDataReceived += delegate (object sender, EventArgs e)
            {
                // Get packet data
                Packet Packet = (Packet)sender;

                // Decode command
                this.Connection.OnCommandReceived?.Invoke(ReadCommand(Packet), EventArgs.Empty);
            };
        }

        // Checks a byte array for 0 bytes, I guess? I dunno
        internal bool ReadStrict(byte[] Data, Int32 Pointer, Int32 Length)
        {
            // Loop through bytes
            Int32 Offset = 0;
            while (Offset < Length)
            {
                // Read byte at offset location
                Int32 Read = Data[Offset + Pointer];
                if (Read == 0) return false;
                Offset += Read;
            }
            return true;
        }

        // Reads a command
        internal Command ReadCommand(Packet Packet)
        {
            // Decode packet information
            byte[] HeadBytes = Packet.Data.SubArray(0, 33);
            bucket_head2 Head = bucket_head2.FromByteArray(HeadBytes);
            byte[] BodyBytes = Packet.Data.SubArray(33, (Int32)Head.PacketSize);

            // Verify packet information
            //byte[] HeadBytes = Encoding.ObjectToByteArray(Head);
            //if (!ReadStrict(HeadBytes, 0, HeadBytes.Length)) return Command;
            if (Head.Signature != LEVIN_SIGNATURE)
            {
                Connection.OnError?.Invoke("Levin signature mismatch", EventArgs.Empty);
                return new Command();
            }
            if (Head.PacketSize > LEVIN_DEFAULT_MAX_PACKET_SIZE)
            {
                Connection.OnError?.Invoke("Levin packet size is too big", EventArgs.Empty);
                return new Command();
            }
            //if (Head.PacketSize > 0 && !ReadStrict(Body, 0, (Int32)Head.PacketSize)) return Command;

            // Write to command
            return new Command
            {
                Source =        Packet.Client,
                CommandCode =   Head.CommandCode,
                Body =          BodyBytes,
                IsNotify =      !Head.ResponseRequired,
                IsResponse =    (Head.Flags & LEVIN_PACKET_RESPONSE) == LEVIN_PACKET_RESPONSE
            };
        }

        // Sends a reply packet
        internal void SendReply(NetworkStream Stream, UInt32 Command, byte[] Body, UInt32 ReturnCode)
        {
            // Create a header
            bucket_head2 Head = new bucket_head2
            {
                Signature =         LEVIN_SIGNATURE,
                PacketSize =        (UInt64)Body.Length,
                ResponseRequired =  false,
                CommandCode =       Command,
                ProtocolVersion =   LEVIN_PROTOCOL_VER_1,
                Flags =             LEVIN_PACKET_RESPONSE,
                ReturnCode =        ReturnCode
            };

            // Combine header and body to a single byte array
            byte[] HeadBytes = Head.ToByteArray();
            byte[] WriteBuffer = new byte[HeadBytes.Length + Body.Length];
            Buffer.BlockCopy(HeadBytes, 0, WriteBuffer, 0, HeadBytes.Length);
            Buffer.BlockCopy(Body, 0, WriteBuffer, HeadBytes.Length, Body.Length);

            // Write data to stream
            Stream.Write(WriteBuffer, 0, WriteBuffer.Length);
        }

        // Sends a packet
        internal void SendMessage(NetworkStream Stream, UInt32 Command, byte[] Body, bool ResponseRequired)
        {
            // Create a header
            bucket_head2 Head = new bucket_head2
            {
                Signature =         LEVIN_SIGNATURE,
                PacketSize =        (UInt64)Body.Length,
                ResponseRequired =  ResponseRequired,
                CommandCode =       Command,
                ProtocolVersion =   LEVIN_PROTOCOL_VER_1,
                Flags =             LEVIN_PACKET_REQUEST
            };

            // Combine header and body to a single byte array
            byte[] HeadBytes = Head.ToByteArray();
            byte[] WriteBuffer = new byte[HeadBytes.Length + Body.Length];
            Buffer.BlockCopy(HeadBytes, 0, WriteBuffer, 0, HeadBytes.Length);
            Buffer.BlockCopy(Body, 0, WriteBuffer, HeadBytes.Length, Body.Length);

            // Write data to stream
            Stream.Write(WriteBuffer, 0, WriteBuffer.Length);
        }

        // Broadcasts a packet to all peers
        internal void BroadcastMessage(UInt32 Command, byte[] Body, bool ResponseRequired)
        {
            // Create a header
            bucket_head2 Head = new bucket_head2
            {
                Signature = LEVIN_SIGNATURE,
                PacketSize = (UInt64)Body.Length,
                ResponseRequired = ResponseRequired,
                CommandCode = Command,
                ProtocolVersion = LEVIN_PROTOCOL_VER_1,
                Flags = LEVIN_PACKET_REQUEST
            };

            // Combine header and body to a single byte array
            byte[] HeadBytes = Head.ToByteArray();
            byte[] WriteBuffer = new byte[HeadBytes.Length + Body.Length];
            Buffer.BlockCopy(HeadBytes, 0, WriteBuffer, 0, HeadBytes.Length);
            Buffer.BlockCopy(Body, 0, WriteBuffer, HeadBytes.Length, Body.Length);

            // Write data to stream
            Connection.Broadcast(WriteBuffer);
        }

        // Handles a handshake request
        internal void HandleHandshakeRequest(Command Command)
        {
            if (!Decode(Command.Body, out Commands.Handshake.Request Request))
            {
                Connection.OnError?.Invoke("Failed to decode COMMAND_HANDSHAKE request", EventArgs.Empty);
                return;
            }

            //m_node.handleNodeData(Request.NodeData, m_context);
            WriteHandshake(new P2pMessage
            {
                Source = Command.Source,
                Type = Command.CommandCode,
                Data = Encoding.ObjectToByteArray(Request.PayloadData)
            }); // enqueue payload info
        }

        // Writes a handshake request
        void WriteHandshake(P2pMessage message) {
            Decode(message.Data, out CORE_SYNC_DATA CoreSync);

            if (message.Source != null)
            {
                // response
                Commands.Handshake.Response res;
                res.NodeData = new object();//m_node.getNodeData();
                res.PayloadData = CoreSync;
                res.LocalPeerList = new List<PeerlistEntry>(); //m_node.getLocalPeerList();
                SendReply(message.Source.GetStream(), (uint)Commands.Handshake.Id, Encode(res), LEVIN_PROTOCOL_RETCODE_SUCCESS);
                //m_node.tryPing(m_context);
            }
            else
            {
                // request
                Commands.Handshake.Request req = new Commands.Handshake.Request
                {
                    //req.NodeData = m_node.getNodeData();
                    PayloadData = CoreSync
                };
                SendReply(message.Source.GetStream(), Commands.Handshake.Id, Encode(req), LEVIN_PACKET_RESPONSE);
                //WriteMessage(makeRequest(Commands.Handshake.Id, Encode(req)));
            }
        }
    }
}
