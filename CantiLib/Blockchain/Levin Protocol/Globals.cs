namespace Canti.Blockchain
{
    // Global levin protocol constants
    internal partial class LevinProtocol
    {
        // Packet flags
        internal const int LEVIN_PACKET_REQUEST = 0x00000001;
        internal const int LEVIN_PACKET_RESPONSE = 0x00000002;

        // Version numbers
        internal const int LEVIN_PROTOCOL_VER_0 = 0;
        internal const int LEVIN_PROTOCOL_VER_1 = 1;

        // Return codes
        internal const int LEVIN_RETCODE_SUCCESS = 0;
        internal const int LEVIN_RETCODE_FAILURE = 1;
    }
}
