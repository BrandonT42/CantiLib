//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    /// <summary>
    /// A configuration class that needs to be specified when initializing a node
    /// </summary>
    public class NetworkConfig
    {
        #region NETWORK

        /// <summary>
        /// This is the byte array value that uniquely identifies the desired network
        /// </summary>
        public byte[] NETWORK_ID { get; set; }

        /// <summary>
        /// (OPTIONAL) A bit of vanity text that will be shown when a node is first initialized
        /// </summary>
        public string ASCII_ART { get; set; }

        #endregion

        #region STORAGE

        /// <summary>
        /// The type of database the node will utilize
        /// </summary>
        public DatabaseType DATABASE_TYPE { get; set; }

        /// <summary>
        /// (OPTIONAL) Specifies a directory where any database files will be saved
        /// </summary>
        public string DATABASE_DIRECTORY { get; set; }

        /// <summary>
        /// The location of the database
        /// </summary>
        public string DATABASE_LOCATION { get; set; }

        #endregion

        #region P2P

        /// <summary>
        /// The default P2P communications port
        /// </summary>
        public int P2P_DEFAULT_PORT { get; set; }

        /// <summary>
        /// The max number of peer connections to be allowed
        /// </summary>
        public int P2P_MAX_PEER_CONNECTIONS { get; set; }

        /// <summary>
        /// The current version of the P2P protocol being used
        /// </summary>
        public int P2P_CURRENT_VERSION { get; set; }

        /// <summary>
        /// The minimum P2P protocol version a peer must be using to be considered valid
        /// </summary>
        public int P2P_MINIMUM_VERSION { get; set; }

        /// <summary>
        /// How often to poll peers for disconnects (in milliseconds)
        /// </summary>
        public int P2P_POLLING_INTERVAL { get; set; }

        /// <summary>
        /// How often to send timed sync packets to check where connected peers are at (in seconds)
        /// </summary>
        public int P2P_TIMED_SYNC_INTERVAL { get; set; }

        #endregion

        #region API

        /// <summary>
        /// The default port to listen for API requests on
        /// </summary>
        public int API_DEFAULT_PORT { get; set; }

        /// <summary>
        /// The maximum number of concurrent API requests allowed at any one time
        /// </summary>
        public int API_MAX_WORKERS { get; set; }

        /// <summary>
        /// The current API protocol version being used
        /// </summary>
        public int API_CURRENT_VERSION { get; set; }

        /// <summary>
        /// The minimum API protocol version a request must be using
        /// </summary>
        public int API_MINIMUM_VERSION { get; set; }

        #endregion

        #region LOGGING

        /// <summary>
        /// (OPTIONAL) A file where all logger output is also written to
        /// </summary>
        public string LOG_FILE { get; set; }

        /// <summary>
        /// What level of logging will be shown
        /// </summary>
        public LogLevel LOG_LEVEL { get; set; }

        /// <summary>
        /// (OPTIONAL) A custom prefix that will be shown before a label name, if showing prefixes
        /// </summary>
        public string CUSTOM_PREFIX { get; set; }

        /// <summary>
        /// The default color for logger output
        /// </summary>
        public ConsoleColor INFO_COLOR { get; set; }

        /// <summary>
        /// The color important messages will be shown in when logging
        /// </summary>
        public ConsoleColor IMPORTANT_COLOR { get; set; }

        /// <summary>
        /// The color debug messages will be shown in when logging
        /// </summary>
        public ConsoleColor DEBUG_COLOR { get; set; }

        /// <summary>
        /// The color warning messages will be shown in when logging
        /// </summary>
        public ConsoleColor WARNING_COLOR { get; set; }

        /// <summary>
        /// The color error messages will be shown in when logging
        /// </summary>
        public ConsoleColor ERROR_COLOR { get; set; }

        #endregion
    }
}
