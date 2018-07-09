using Canti.Data;
using System;

namespace Canti.CryptoNote.P2P
{
    internal struct NetworkAddress
    {
        internal uint IpAddress { get; set; }
        internal uint Port { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("ip", IpAddress);
            Storage.AddEntry("port", Port);
            return Storage.Serialize(false);
        }
    };

    internal struct PeerlistEntry
    {
        internal NetworkAddress Address { get; set; }
        internal ulong Id { get; set; }
        internal ulong LastSeen { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("address", Address.Serialize());
            Storage.AddEntry("id", Id);
            Storage.AddEntry("last_seen", LastSeen);
            return Storage.Serialize(false);
        }
    };

    internal struct NodeData
    {
        internal Guid NetworkId { get; set; }
        internal byte Version { get; set; }
        internal ulong PeerId { get; set; }
        internal ulong LocalTime { get; set; }
        internal uint Port { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("network_id", NetworkId);
            Storage.AddEntry("version", Version);
            Storage.AddEntry("peer_id", PeerId);
            Storage.AddEntry("local_time", LocalTime);
            Storage.AddEntry("my_port", Port);
            return Storage.Serialize(false);
        }
    }
}
