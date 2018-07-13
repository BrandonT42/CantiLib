using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.Blockchain
{
    [Serializable]
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
}
