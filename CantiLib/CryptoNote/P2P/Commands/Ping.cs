using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P.Commands
{
    class Ping
    {
        internal static UInt32 Id = LevinProtocol.P2P_COMMANDS_POOL_BASE + 3;
        internal string PING_OK_RESPONSE_STATUS_TEXT = "OK";

        [Serializable]
        struct Request
        {
            /*actually we don't need to send any real data*/
            void Serialize() { }
        };

        [Serializable]
        struct Response
        {
            string Status;
            UInt64 PeerId;

            void Serialize()
            {
                //KV_MEMBER(status);
                //KV_MEMBER(PeerId);
            }
        };
    }
}
