//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    public class NetworkConfig
    {
        #region NETWORK

        // This is the byte array value that uniquely identifies the desired network
        public byte[] NETWORK_ID { get; set; }

        // An optional bit of vanity text that will be shown when a node is launched
        public string ASCII_ART { get; set; }

        #endregion

        #region P2P

        // The default P2P communications port
        public int P2P_DEFAULT_PORT { get; set; }

        // The max number of peer connections to be allowed
        public int P2P_MAX_PEER_CONNECTIONS { get; set; }

        // The current version of the P2P protocol being used
        public int P2P_CURRENT_VERSION { get; set; }

        // The minimum P2P protocol version a peer must be using to be considered valid
        public int P2P_MINIMUM_VERSION { get; set; }

        // How often to poll peers for disconnects (in milliseconds)
        public int P2P_POLLING_INTERVAL { get; set; }

        // How often to send timed sync packets to check where connected peers are at (in seconds)
        public int P2P_TIMED_SYNC_INTERVAL { get; set; }

        #endregion

        #region API

        // The default port to listen for API requests on
        public int API_DEFAULT_PORT { get; set; }

        // The maximum number of concurrent API requests allowed at any one time
        public int API_MAX_WORKERS { get; set; }

        // The current API protocol version being used
        public int API_CURRENT_VERSION { get; set; }

        // The minimum API protocol version a request must be using
        public int API_MINIMUM_VERSION { get; set; }

        #endregion

        #region LOGGING

        // An optional log file where logger output will be saved to
        public string LOG_FILE { get; set; }

        // How much logging will be shown (ranges in value from -2 to 3)
        public LogLevel LOG_LEVEL { get; set; }

        // The default color for logger output
        public ConsoleColor INFO_COLOR { get; set; }

        // The color important messages will be shown in when logging
        public ConsoleColor IMPORTANT_COLOR { get; set; }

        // The color debug messages will be shown in when logging
        public ConsoleColor DEBUG_COLOR { get; set; }

        // The color warnings will be shown in when logging
        public ConsoleColor WARNING_COLOR { get; set; }

        // The color errors will be shown in when logging
        public ConsoleColor ERROR_COLOR { get; set; }

        #endregion
    }
}
