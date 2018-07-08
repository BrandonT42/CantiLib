using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    internal struct NetworkAddress
    {
        internal uint IpAddress;
        internal uint Port;
    };

    internal struct PeerlistEntry
    {
        internal NetworkAddress Address;
        internal ulong Id;
        internal ulong LastSeen;
    };

    internal struct NodeData
    {
        internal Guid NetworkId;
        internal byte Version;
        internal ulong LocalTime;
        internal uint Port;
        internal ulong PeerId;
    }
}
