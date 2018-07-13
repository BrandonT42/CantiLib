namespace Canti.Blockchain.Commands
{
    internal class Ping
    {
        // Command ID
        internal const int Id = GlobalsConfig.LEVIN_COMMANDS_BASE + 3;

        // Outgoing request structure
        internal struct Request : ICommandRequestBase
        {
            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // No data is needed for this request
                return new byte[0];
            }
        }

        // Incoming response structure
        internal struct Response : ICommandResponseBase<Response>
        {
            // Variables
            internal string Status { get; set; }
            internal ulong PeerId { get; set; }

            // Deseriaizes response data
            public static Response Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Response
                {
                    Status = (string)Storage.GetEntry("status"),
                    PeerId = (ulong)Storage.GetEntry("peer_id")
                };
            }
        }

        // Process incoming command instance
        internal static void Invoke(LevinProtocol Context, LevinPeer Peer, Command Command)
        {
            // Command is a request
            if (!Command.IsResponse)
            {
                // TODO: Do something
            }

            // Command is a response
            else
            {
                // TODO: Do something
            }
        }
    }
}
