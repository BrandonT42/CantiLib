using System;
using System.Net.Sockets;

namespace Canti.CryptoNote.P2P
{
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
        internal UInt32 Type { get; set; }
        internal byte[] Data { get; set; }
    }
    /*struct CommandHandshake
    {
        internal static Int32 ID = Globals.P2P_COMMANDS_POOL_BASE + 1;

        struct Request
        {
            basic_node_data node_data;
            CORE_SYNC_DATA payload_data;

            void serialize(ISerializer& s)
            {
                KV_MEMBER(node_data)
              KV_MEMBER(payload_data)
            }

        };

        struct Response
        {
            basic_node_data node_data;
            CORE_SYNC_DATA payload_data;
            std::list<PeerlistEntry> local_peerlist;

            void serialize(ISerializer& s)
            {
                KV_MEMBER(node_data)
              KV_MEMBER(payload_data)
              serializeAsBinary(local_peerlist, "local_peerlist", s);
            }
        };
    }*/
}
