using Canti.Data;

namespace Canti.Blockchain
{
    class GlobalsConfig
    {
        // Network
        internal static string NETWORK_ID               = Encoding.ByteArrayToString(new byte[] { 0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41, 0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9 });
        internal static string[] SEED_NODES             = {
                                                              "174.138.68.141:11897", // Rock
                                                              "145.239.88.119:11999", // Cision
                                                              "142.44.242.106:11897", // Tom
                                                              "165.227.252.132:11897" // iburnmycd
                                                          };

        // P2P
        internal const int     P2P_DEFAULT_PORT         = 8090;

        // Levin Protocol
        internal const ulong   LEVIN_SIGNATURE          = 0x0101010101012101UL; // Bender's Nightmare
        internal static byte   LEVIN_VERSION            = LevinProtocol.LEVIN_PROTOCOL_VER_1;
        internal const int     LEVIN_DEFAULT_TIMEOUT    = 0;
        internal const int     LEVIN_MAX_PACKET_SIZE    = 100000000; // 100 MB
        internal const int     LEVIN_COMMANDS_BASE      = 1000; // Levin protocol packet command codes are added to this value

        // Portable Storage
        internal const uint    STORAGE_SIGNATUREA       = 0x01011101; // Bender's Nightmare (Part 1)
        internal const uint    STORAGE_SIGNATUREB       = 0x01020101; // Bender's Nightmare (Part 2) 
        internal const byte    STORAGE_FORMAT_VERSION   = 1;

        // CryptoNote Protocol
        internal const int     CRYPTONOTE_COMMANDS_BASE = 2000; // Cryptonote protocol packet command codes are added to this value
    }
}
