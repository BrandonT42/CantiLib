using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.Blockchain
{
    [Serializable]
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
}
