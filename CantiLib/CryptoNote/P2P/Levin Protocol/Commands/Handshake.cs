using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.CryptoNote.P2P.Commands
{
    internal class CommandHandshake
    {
        // Command ID
        internal const int Id = Globals.P2P_COMMANDS_POOL_BASE + 1;

        // Outgoing request structure
        internal struct Request : ICommandRequestBase
        {
            // Variables
            internal NodeData NodeData { get; set; }
            internal CoreSyncData PayloadData { get; set; }

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("node_data", NodeData);
                Storage.AddEntry("payload_data", PayloadData);

                // Return serialized byte array
                return Storage.Serialize();
            }
        }

        // Incoming response structure
        internal struct Response : ICommandResponseBase<Response>
        {
            // Variables
            internal NodeData NodeData { get; set; }
            internal CoreSyncData Data { get; set; }
            internal PeerlistEntry[] LocalPeerlistNew { get; set; }

            // Serializes response data
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                if (!Storage.AddEntry("basic_node_data", NodeData)) Console.WriteLine("Failed to add basic_node_data");
                if (!Storage.AddEntry("core_sync_data", Data)) Console.WriteLine("Failed to add core_sync_data");
                if (!Storage.AddEntry("local_peerlist_new", LocalPeerlistNew)) Console.WriteLine("Failed to add local_peerlist_new");

                // Return serialized byte array
                return Storage.Serialize();
            }

            // Deseriaizes response data
            public Response Deserialize(byte[] Data)
            {
                // Deserialize data
                //Data = Encoding.DecompressByteArray(Data);
                return (Response)Encoding.DecodeObject(Data);

                // Populate and return new response
                /*return new Response
                {
                    
                };*/
            }
        }
    }
}
