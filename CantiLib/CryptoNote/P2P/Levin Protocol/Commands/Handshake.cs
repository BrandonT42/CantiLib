using Canti.Data;
using System;

namespace Canti.CryptoNote.P2P.Commands
{
    internal class CommandHandshake
    {
        // Command ID
        internal const int Id = GlobalsConfig.LEVIN_COMMANDS_BASE + 1;

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

            // Deseriaizes response data
            public static Request Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Request
                {
                    NodeData = NodeData.Deserialize(Storage.GetEntry("node_data")),
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data"))
                };
            }
        }

        // Incoming response structure
        internal struct Response : ICommandResponseBase<Response>
        {
            // Variables
            internal NodeData NodeData { get; set; }
            internal CoreSyncData PayloadData { get; set; }
            internal PeerlistEntry[] LocalPeerlist { get; set; }

            // Serializes response data
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntry("node_data", NodeData);
                Storage.AddEntry("payload_data", PayloadData);
                Storage.AddEntryAsBinary("local_peerlist", LocalPeerlist);

                // Return serialized byte array
                return Storage.Serialize();
            }

            // Deseriaizes response data
            public static Response Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Response
                {
                    NodeData = NodeData.Deserialize(Storage.GetEntry("node_data")),
                    PayloadData = CoreSyncData.Deserialize(Storage.GetEntry("payload_data")),
                    LocalPeerlist = Storage.DeserializeArrayFromBinary<PeerlistEntry>("local_peerlist")
                };
            }
        }

        // Process incoming command instance
        internal static void HandleCommand(Command Command, Utilities.Logger Logger = null)
        {
            // Command is a request
            if (!Command.IsResponse)
            {
                // Deserialize request
                Request Request = Request.Deserialize(Command.Data);

                // TODO
            }

            // Command is a response
            else
            {
                // Deserialize response
                Response Response = Response.Deserialize(Command.Data);

                // debug
                if (Logger != null)
                {
                    Logger.LogLevel = Utilities.Level.DEBUG;
                    Logger.Log(Utilities.Level.DEBUG, "Received Handshake Response:");
                    Logger.Log(Utilities.Level.DEBUG, "- Node Data:");
                    Logger.Log(Utilities.Level.DEBUG, "  - Network ID: {0}", Encoding.StringToHexString(Response.NodeData.NetworkId));
                    Logger.Log(Utilities.Level.DEBUG, "  - Peer ID: {0}", Response.NodeData.PeerId);
                    Logger.Log(Utilities.Level.DEBUG, "  - Version: {0}", Response.NodeData.Version);
                    Logger.Log(Utilities.Level.DEBUG, "  - Local Time: {0}", Response.NodeData.LocalTime);
                    Logger.Log(Utilities.Level.DEBUG, "  - Port: {0}", Response.NodeData.Port);
                    Logger.Log(Utilities.Level.DEBUG, "- Core Sync Data:");
                    Logger.Log(Utilities.Level.DEBUG, "  - Current Height: {0}", Response.PayloadData.CurrentHeight);
                    Logger.Log(Utilities.Level.DEBUG, "  - Top ID: {0}", Encoding.StringToHexString(Response.PayloadData.TopId));
                    Logger.Log(Utilities.Level.DEBUG, "- Local Peerlist:");
                    Logger.Log(Utilities.Level.DEBUG, "  - Entries: {0}", Response.LocalPeerlist.Length);
                }

                // TODO
            }
        }
    }
}
