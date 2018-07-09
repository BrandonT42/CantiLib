using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    internal class Globals
    {
        internal const int P2P_COMMANDS_POOL_BASE = 1000;
        internal static Guid NETWORK_ID = new Guid(new byte[] { 0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41, 0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9 });
    }
}
