using Canti.Data;
using System;
using System.Net.Sockets;

namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        internal const Int64 LEVIN_SIGNATURE = 0x0101010101012101;      // Bender's Nightmare
        internal const Int32 LEVIN_PACKET_REQUEST = 0x00000001;
        internal const Int32 LEVIN_PACKET_RESPONSE = 0x00000002;
        internal const Int32 LEVIN_DEFAULT_MAX_PACKET_SIZE = 100000000; // 100MB by Default
        internal const Int32 LEVIN_PROTOCOL_VER_1 = 1;
        internal const Int32 LEVIN_PROTOCOL_RETCODE_SUCCESS = 1;
        internal const Int32 P2P_COMMANDS_POOL_BASE = 1000;

        // Decodes a byte array to a specified object type
        internal static bool Decode<T>(byte[] Body, out T Value)
        {
            try
            {
                Value = (T)Encoding.DecodeObject(Body);
                return true;
            }
            catch
            {
                Value = default(T);
                return false;
            }
        }

        // Encodes an object to a byte array
        internal static byte[] Encode(object Value)
        {
            return Encoding.ObjectToByteArray(Value);
        }
    }

    public struct Packet
    {
        public TcpClient Client;
        public byte[] Data;
        public Packet(TcpClient Client, byte[] Data)
        {
            this.Client = Client;
            this.Data = Data;
        }
    }

    internal struct P2pMessage
    {
        internal TcpClient Source { get; set; }
        internal UInt32 Type { get; set; }
        internal byte[] Data { get; set; }
    }

    struct NetworkAddress
    {
        UInt32 IP;
        UInt32 Port;
    };

    struct PeerlistEntry
    {
        NetworkAddress Address;
        UInt64 Id;
        UInt64 LastSeen;
    };

    struct ConnectionEntry
    {
        NetworkAddress Address;
        UInt64 Id;
        bool IsIncoming;
    };

    [Serializable]
    internal struct Command
    {
        internal TcpClient Source { get; set; }
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

    [Serializable]
    struct CORE_SYNC_DATA
    {
        UInt32 CurrentHeight;
        string TopId;

        void Serialize()
        {
            //KV_MEMBER(current_height)
            //KV_MEMBER(top_id)
        }
    };
}
