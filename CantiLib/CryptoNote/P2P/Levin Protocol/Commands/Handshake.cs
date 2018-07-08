using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P.Commands
{
    internal class CommandHandshake
    {
        // Command ID
        internal const int Id = Globals.P2P_COMMANDS_POOL_BASE + 1;

        // Outgoing request structure
        internal struct Request : ICommandRequestBase
        {
            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                
                return new byte[0];
            }
        }

        // Incoming response structure
        internal struct Response : ICommandResponseBase<Response>
        {
            // Variables
            internal NodeData NodeData;
            internal CoreSyncData Data;
            internal List<PeerlistEntry> LocalPeerlistNew;

            // Deseriaizes response data
            public Response Deserialize(byte[] Data)
            {
                // Deserialize data


                // Populate and return new response
                return new Response
                {
                    
                };
            }
        }
    }
}
