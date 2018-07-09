using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.CryptoNote.P2P
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

        // Error codes
        internal enum ErrorCode : int
        {
            LEVIN_OK = 0,
            LEVIN_ERROR_CONNECTION = -1,
            LEVIN_ERROR_CONNECTION_NOT_FOUND = -2,
            LEVIN_ERROR_CONNECTION_DESTROYED = -3,
            LEVIN_ERROR_CONNECTION_TIMEDOUT = -4,
            LEVIN_ERROR_CONNECTION_NO_DUPLEX_PROTOCOL = -5,
            LEVIN_ERROR_CONNECTION_HANDLER_NOT_DEFINED = -6,
            LEVIN_ERROR_FORMAT = -7
        }
    }
}
