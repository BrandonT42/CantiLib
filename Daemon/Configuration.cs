//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    partial class Daemon
    {
        // This is the network configuration we will pass to our node instance when creating it
        private static readonly NetworkConfig Configuration = new NetworkConfig
        {
            #region Network

            NETWORK_ID = new byte[] {
                0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41,
                0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9
            },

            ASCII_ART = "" +
            "\n                                                                            \n" +
            "████████╗██╗  ██╗██████╗ ████████╗██╗    ██████╗ █████╗ █████╗ ██╗███╗   ██╗\n" +
            "╚══██╔══╝██║  ██║██╔══██╗╚══██╔══╝██║    ██╔═══╝██╔═══╝██╔══██╗██║████╗  ██║\n" +
            "   ██║   ██║  ██║██████╔╝   ██║   ██║    ████╗  ██║    ██║  ██║██║██╔██╗ ██║\n" +
            "   ██║   ██║  ██║██╔══██╗   ██║   ██║    ██╔═╝  ██║    ██║  ██║██║██║╚██╗██║\n" +
            "   ██║   ╚█████╔╝██║  ██║   ██║   ██████╗██████╗╚█████╗╚█████╔╝██║██║ ╚████║\n" +
            "   ╚═╝    ╚════╝ ╚═╝  ╚═╝   ╚═╝   ╚═════╝╚═════╝ ╚════╝ ╚════╝ ╚═╝╚═╝  ╚═══╝\n",

            #endregion

            #region P2P

            P2P_DEFAULT_PORT = 8090,
            P2P_MAX_PEER_CONNECTIONS = 5,
            P2P_CURRENT_VERSION = 5,
            P2P_MINIMUM_VERSION = 4,
            P2P_POLLING_INTERVAL = 100,
            P2P_TIMED_SYNC_INTERVAL = 6,

            #endregion

            #region Rest API

            API_DEFAULT_PORT = 8091,
            API_MAX_WORKERS = 5,
            API_CURRENT_VERSION = 1,
            API_MINIMUM_VERSION = 0,

            #endregion

            #region Logger

            LOG_FILE = "cs-turtlecoin.log",
            LOG_LEVEL = LogLevel.MAX,
            INFO_COLOR = ConsoleColor.White,
            IMPORTANT_COLOR = ConsoleColor.Green,
            DEBUG_COLOR = ConsoleColor.DarkGray,
            WARNING_COLOR = ConsoleColor.Yellow,
            ERROR_COLOR = ConsoleColor.Red,

            #endregion
        };
    }
}
