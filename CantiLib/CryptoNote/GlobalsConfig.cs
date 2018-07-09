using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.CryptoNote
{
    class GlobalsConfig
    {
        // P2P
        internal const int     P2P_COMMANDS_POOL_BASE  = 1000;
        internal const int     P2P_DEFAULT_PORT        = 8090;
        internal static string P2P_NETWORK_ID          = Encoding.ByteArrayToString(new byte[] { 0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41, 0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9 });

        // Levin Protocol
        internal const ulong   LEVIN_SIGNATURE         = 0x0101010101012101UL; // Bender's Nightmare
        internal const int     LEVIN_DEFAULT_TIMEOUT   = 0;
        internal const int     LEVIN_MAX_PACKET_SIZE   = 100000000; // 100 MB
    }
}
