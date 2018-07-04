using System;
using System.Collections.Generic;

namespace Canti.CryptoNote.P2P.Commands
{
    class Handshake
    {
        internal static UInt32 Id = LevinProtocol.P2P_COMMANDS_POOL_BASE + 1;

        [Serializable]
        internal struct Request
        {
            internal object NodeData;
            internal CORE_SYNC_DATA PayloadData;

            internal void Serialize()
            {
                //KV_MEMBER(node_data)
                //KV_MEMBER(payload_data)
            }

        };

        [Serializable]
        internal struct Response
        {
            internal object NodeData;
            internal CORE_SYNC_DATA PayloadData;
            internal IList<PeerlistEntry> LocalPeerList;

            internal void Serialize()
            {
                //KV_MEMBER(node_data)
                //KV_MEMBER(payload_data)
                //serializeAsBinary(local_peerlist, "local_peerlist", s);
            }
        };
    }
}
