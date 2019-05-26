//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using Canti.CryptoNote;
using System;
using static Canti.Utils;

namespace CantiTests
{
    partial class Program
    {
        private static readonly NetworkConfig TrtlConfig = new NetworkConfig
        {
            #region Network

            NETWORK_ID = new byte[] {
                0xb5, 0x0c, 0x4a, 0x6c, 0xcf, 0x52, 0x57, 0x41,
                0x65, 0xf9, 0x91, 0xa4, 0xb6, 0xc1, 0x43, 0xe9
            },

            #endregion

            #region STORAGE

            DATABASE_TYPE = DatabaseType.SQLITE,
            DATABASE_DIRECTORY = GetAppDataPath("TurtleCoin"),
            DATABASE_LOCATION = "CS-TurtleCoin.db",

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

            LOG_FILE = "trtl.log",
            CUSTOM_PREFIX = "TRTL",
            INFO_COLOR = ConsoleColor.White,
            IMPORTANT_COLOR = ConsoleColor.Green,
            DEBUG_COLOR = ConsoleColor.DarkGray,
            WARNING_COLOR = ConsoleColor.Yellow,
            ERROR_COLOR = ConsoleColor.Red,

            #endregion
        };

        private static readonly NetworkConfig AthxConfig = new NetworkConfig
        {
            #region Network

            NETWORK_ID = new byte[] {
                0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
                0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xf0
            },

            #endregion

            #region STORAGE

            DATABASE_TYPE = DatabaseType.SQLITE,
            DATABASE_DIRECTORY = GetAppDataPath("Athena"),
            DATABASE_LOCATION = "CS-Athena.db",

            #endregion

            #region P2P

            P2P_DEFAULT_PORT = 8092,
            P2P_MAX_PEER_CONNECTIONS = 5,
            P2P_CURRENT_VERSION = 5,
            P2P_MINIMUM_VERSION = 4,
            P2P_POLLING_INTERVAL = 100,
            P2P_TIMED_SYNC_INTERVAL = 6,

            #endregion

            #region Rest API

            API_DEFAULT_PORT = 8093,
            API_MAX_WORKERS = 5,
            API_CURRENT_VERSION = 1,
            API_MINIMUM_VERSION = 0,

            #endregion

            #region Logger

            LOG_FILE = "athx.log",
            CUSTOM_PREFIX = "ATHX",
            INFO_COLOR = ConsoleColor.Cyan,
            IMPORTANT_COLOR = ConsoleColor.Magenta,
            DEBUG_COLOR = ConsoleColor.Gray,
            WARNING_COLOR = ConsoleColor.DarkYellow,
            ERROR_COLOR = ConsoleColor.DarkRed,

            #endregion
        };
    }
}
