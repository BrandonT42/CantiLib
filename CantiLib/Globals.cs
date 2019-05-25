//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    public sealed class Globals
    {
        #region Configuration

        #region Network

        public static byte[] NETWORK_ID = new byte[] {
            0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41,
            0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9
        };

        public static string ASCII_ART = "" +
            "\n                                                                            \n" +
            "████████╗██╗  ██╗██████╗ ████████╗██╗    ██████╗ █████╗ █████╗ ██╗███╗   ██╗\n" +
            "╚══██╔══╝██║  ██║██╔══██╗╚══██╔══╝██║    ██╔═══╝██╔═══╝██╔══██╗██║████╗  ██║\n" +
            "   ██║   ██║  ██║██████╔╝   ██║   ██║    ████╗  ██║    ██║  ██║██║██╔██╗ ██║\n" +
            "   ██║   ██║  ██║██╔══██╗   ██║   ██║    ██╔═╝  ██║    ██║  ██║██║██║╚██╗██║\n" +
            "   ██║   ╚█████╔╝██║  ██║   ██║   ██████╗██████╗╚█████╗╚█████╔╝██║██║ ╚████║\n" +
            "   ╚═╝    ╚════╝ ╚═╝  ╚═╝   ╚═╝   ╚═════╝╚═════╝ ╚════╝ ╚════╝ ╚═╝╚═╝  ╚═══╝\n";

        #endregion

        #region P2P

        public const int P2P_DEFAULT_PORT = 8090;
        public const int P2P_MAX_PEER_CONNECTIONS = 5;
        public const int P2P_CURRENT_VERSION = 5;
        public const int P2P_MINIMUM_VERSION = 4;

        public const int P2P_POLLING_INTERVAL = 100;

        public const int P2P_TIMED_SYNC_INTERVAL = 6;

        #endregion

        #region Rest API

        public const int API_DEFAULT_PORT = 8091;
        public const int API_MAX_WORKERS = 5;
        public const int API_CURRENT_VERSION = 1;
        public const int API_MINIMUM_VERSION = 0;

        #endregion

        #region Logger

        public const ConsoleColor InfoColor = ConsoleColor.White;
        public const ConsoleColor ImportantColor = ConsoleColor.Green;
        public const ConsoleColor DebugColor = ConsoleColor.DarkGray;
        public const ConsoleColor WarningColor = ConsoleColor.Yellow;
        public const ConsoleColor ErrorColor = ConsoleColor.Red;

        #endregion

        #endregion
    }
}
