using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    // Header for levin protocol requests
    internal struct BucketHead2
    {
        internal ulong Signature { get; set; }
        internal ulong PacketSize { get; set; }
        internal bool ResponseRequired { get; set; }
        internal uint CommandCode { get; set; }
        internal int ReturnCode { get; set; }
        internal uint Flags { get; set; }
        internal uint ProtocolVersion { get; set; }
    };
}
