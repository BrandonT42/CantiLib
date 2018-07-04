using System;
using System.Net.Sockets;
using Canti.Data;

// TODO:
// - Handshake
// - Figure out that ReadStrict shit to see what it's wanting
namespace Canti.CryptoNote.P2P
{
    internal class LevinProtocol
    {
        // Constants
        const Int64 LEVIN_SIGNATURE = 0x0101010101012101;      // Bender's Nightmare
        const Int32 LEVIN_PACKET_REQUEST = 0x00000001;
        const Int32 LEVIN_PACKET_RESPONSE = 0x00000002;
        const Int32 LEVIN_DEFAULT_MAX_PACKET_SIZE = 100000000; // 100MB by Default
        const Int32 LEVIN_PROTOCOL_VER_1 = 1;
        const Int32 LEVIN_PROTOCOL_RETCODE_SUCCESS = 1;

        // Variables
        private Server Connection;

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Link server
            this.Connection = Connection;

            // Add an event handler to the onDataReceived event
            this.Connection.OnDataReceived += delegate (object Request, EventArgs e)
            {
                // Get packet data
                Packet Packet = (Packet)Request;

                // Decode bytes
                byte[] HeadBytes = Packet.Data.SubArray(0, 33);
                bucket_head2 Head = bucket_head2.FromByteArray(HeadBytes);
                byte[] BodyBytes = Packet.Data.SubArray(33, (Int32)Head.PacketSize);

                // Decode command
                this.Connection.OnCommandReceived?.Invoke(ReadCommand(Head, BodyBytes), EventArgs.Empty);
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
        internal Command ReadCommand(bucket_head2 Head, byte[] Body)
        {
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
                CommandCode =   Head.CommandCode,
                Body =          Body,
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
    }

    [Serializable]
    internal struct Command
    {
        internal UInt32 CommandCode { get; set; }
        internal bool IsNotify { get; set; }
        internal bool IsResponse { get; set; }
        internal byte[] Body { get; set; }
        internal bool NeedsReply { get { return !(IsNotify || IsResponse); } }
    }

    [Serializable]
    internal struct bucket_head2
    {
        internal UInt64 Signature { get; set; }
        internal UInt64 PacketSize { get; set; }
        internal bool ResponseRequired { get; set; }
        internal UInt32 CommandCode { get; set; }
        internal UInt32 ReturnCode { get; set; }
        internal UInt32 Flags { get; set; }
        internal UInt32 ProtocolVersion { get; set; }
        internal byte[] ToByteArray()
        {
            Int32 Offset = 0;
            byte[] Output = new byte[33];
            byte[] SignatureBytes = BitConverter.GetBytes(Signature);
            Buffer.BlockCopy(SignatureBytes, 0, Output, Offset, SignatureBytes.Length);
            Offset += SignatureBytes.Length;
            byte[] PacketSizeBytes = BitConverter.GetBytes(PacketSize);
            Buffer.BlockCopy(PacketSizeBytes, 0, Output, Offset, PacketSizeBytes.Length);
            Offset += PacketSizeBytes.Length;
            byte[] ResponseRequiredBytes = BitConverter.GetBytes(ResponseRequired);
            Buffer.BlockCopy(ResponseRequiredBytes, 0, Output, Offset, ResponseRequiredBytes.Length);
            Offset += ResponseRequiredBytes.Length;
            byte[] CommandCodeBytes = BitConverter.GetBytes(CommandCode);
            Buffer.BlockCopy(CommandCodeBytes, 0, Output, Offset, CommandCodeBytes.Length);
            Offset += CommandCodeBytes.Length;
            byte[] ReturnCodeBytes = BitConverter.GetBytes(ReturnCode);
            Buffer.BlockCopy(ReturnCodeBytes, 0, Output, Offset, ReturnCodeBytes.Length);
            Offset += ReturnCodeBytes.Length;
            byte[] FlagsBytes = BitConverter.GetBytes(Flags);
            Buffer.BlockCopy(FlagsBytes, 0, Output, Offset, FlagsBytes.Length);
            Offset += FlagsBytes.Length;
            byte[] ProtocolVersionBytes = BitConverter.GetBytes(ProtocolVersion);
            Buffer.BlockCopy(ProtocolVersionBytes, 0, Output, Offset, ProtocolVersionBytes.Length);
            return Output;
        }
        internal static bucket_head2 FromByteArray(byte[] Data)
        {
            return new bucket_head2
            {
                Signature = BitConverter.ToUInt64(Data.SubArray(0, 8)),
                PacketSize = BitConverter.ToUInt64(Data.SubArray(8, 8)),
                ResponseRequired = BitConverter.ToBoolean(Data.SubArray(16, 1)),
                CommandCode = BitConverter.ToUInt32(Data.SubArray(17, 4)),
                ReturnCode = BitConverter.ToUInt32(Data.SubArray(21, 4)),
                Flags = BitConverter.ToUInt32(Data.SubArray(25, 4)),
                ProtocolVersion = BitConverter.ToUInt32(Data.SubArray(29, 4))
            };
        }
    };

    enum LevinError : Int32
    {
        OK = 0,
        ERROR_CONNECTION = -1,
        ERROR_CONNECTION_NOT_FOUND = -2,
        ERROR_CONNECTION_DESTROYED = -3,
        ERROR_CONNECTION_TIMEDOUT = -4,
        ERROR_CONNECTION_NO_DUPLEX_PROTOCOL = -5,
        ERROR_CONNECTION_HANDLER_NOT_DEFINED = -6,
        ERROR_FORMAT = -7
    };
}
