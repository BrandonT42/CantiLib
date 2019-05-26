//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    /// <summary>
    /// A CryptoNote network node
    /// </summary>
    public sealed partial class Node : INode
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// The logger object this node will use to output any information
        /// </summary>
        public Logger Logger { get; set; }

        /// <summary>
        /// The port this node's P2P server will bind to
        /// </summary>
        public int P2pPort { get; set; }

        /// <summary>
        /// The port this node's API server will bind to
        /// </summary>
        public int ApiPort { get; set; }

        /// <summary>
        /// A unique identifier for this node
        /// </summary>
        public ulong Id { get; private set; }

        #endregion

        #region Internal

        // Holds configuration for everything on the network
        internal NetworkConfig Globals { get; private set; }

        #endregion

        #region Private

        // The location of the database being used
        private string DatabaseLocation { get; set; }

        #endregion

        #region Private

        // The database this node will use for storage
        private IDatabase Database { get; set; }

        // This node's blockchain storage handler
        private BlockchainStorage Blockchain { get; set; }

        // This node's P2P server
        private P2pServer P2pServer { get; set; }

        // This node's API server
        private ApiServer ApiServer { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Starts this node and any associated threads
        /// </summary>
        /// <returns>True if started successfully</returns>
        public bool Start()
        {
            // Start blockchain handler
            Logger.WriteLine("Starting blockchain handler...");
            Logger?.WriteLine($"Database location: {DatabaseLocation}");
            try { Blockchain.Start(Database); }
            catch (Exception e)
            {
                Logger?.Error($"Could not start blockchain handler: {e.Message}");
                return false;
            }
            Logger?.WriteLine($"Blockchain handler started");

            // Start P2P server
            Logger?.WriteLine("Starting P2P server...");
            if (P2pPort == 0) P2pPort = Globals.P2P_DEFAULT_PORT;
            try { P2pServer.Start(P2pPort, Globals.P2P_POLLING_INTERVAL); }
            catch (Exception e)
            {
                Logger?.Error($"Could not start P2P server: {e.Message}");
                return false;
            }
            Logger?.WriteLine($"P2P server started on port {P2pServer.Port}");

            // Start API server
            Logger?.WriteLine("Starting API server...");
            if (ApiPort == 0) ApiPort = Globals.API_DEFAULT_PORT;
            try { ApiServer.Start(ApiPort); }
            catch (Exception e)
            {
                Logger?.Error($"Could not start API server: {e.Message}");
                return false;
            }
            Logger?.WriteLine($"API server started on port {ApiServer.Port}");

            // Start sync process
            Logger?.WriteLine("Starting sync process...");
            StartSync();

            // Start is completed
            Logger?.Important($"Node initialized with ID {Id}");
            return true;
        }

        /// <summary>
        /// Stops this node and waits for all associated threads to exit
        /// </summary>
        public void Stop()
        {
            // Stop API server
            Logger?.WriteLine("Stopping API server...");
            ApiServer.Stop();

            // Stop P2P server
            Logger?.WriteLine("Stopping P2P server...");
            P2pServer.Stop();

            // Dispose of all connected peers
            Logger?.WriteLine("Disposing of peers...");
            foreach (var Peer in PeerList)
            {
                Peer.Dispose();
            }
            Logger?.Important("Node stopped.");
        }

        /// <summary>
        /// Adds a peer to the connection queue to be accepted when space is available
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        public void AddPeer(string Address, int Port)
        {
            // Add peer to server
            P2pServer.AddPeer(Address, Port);
        }

        /// <summary>
        /// Adds a peer, ignoring the current connection queue, and booting another peer if needed
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        public void ForceAddPeer(string Address, int Port)
        {
            // Force-add peer to server
            P2pServer.ForceAddPeer(Address, Port);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this node with the specified network configuration
        /// </summary>
        /// <param name="Configuration">A class containing all global information this node needs to operate</param>
        public Node(NetworkConfig Configuration)
        {
            // Assign configuration
            Globals = Configuration;

            // Setup logger
            Logger = new Logger()
            {
                LogFile = Globals.LOG_FILE,
                LogLevel = Globals.LOG_LEVEL,
                CustomPrefix = Globals.CUSTOM_PREFIX,
                InfoColor = Globals.INFO_COLOR,
                ImportantColor = Globals.IMPORTANT_COLOR,
                DebugColor = Globals.DEBUG_COLOR,
                WarningColor = Globals.WARNING_COLOR,
                ErrorColor = Globals.ERROR_COLOR
            };

            // Show ascii art, strictly vanity
            if (!string.IsNullOrEmpty(Globals.ASCII_ART))
            {
                Logger?.Important(Globals.ASCII_ART);
            }
            Logger.ShowPrefix = true;

            // Setup our database
            Logger?.WriteLine("Setting up local storage...");
            switch (Globals.DATABASE_TYPE)
            {
                case DatabaseType.SQLITE:
                    DatabaseLocation = CombinePath(Globals.DATABASE_DIRECTORY, Globals.DATABASE_LOCATION);
                    Database = new SQLite(DatabaseLocation);
                    break;
                default:
                    throw new ArgumentException("Invalid or non-specified database type");
            }

            // Setup our blockchain handler
            Logger?.WriteLine("Setting up blockchain handler...");
            Blockchain = new BlockchainStorage();

            // Create our P2P server
            Logger?.WriteLine("Setting up P2P server...");
            P2pServer = new P2pServer(Globals.P2P_MAX_PEER_CONNECTIONS);

            // Assign P2P event handlers
            Logger?.WriteLine("Assigning P2P callbacks...");
            AssignCallbacks();

            // Setup our API server
            Logger?.WriteLine("Setting up API server...");
            ApiServer = new ApiServer(Globals.API_MAX_WORKERS)
            {
                Logger = Logger
            };

            // Create an API context
            Logger?.WriteLine("Assigning API method context...");
            ApiServer.AssignMethodContext(new ApiMethods(this));

            // Setup our peer list
            Logger?.WriteLine("Setting up local peer list...");
            SetupPeerList();

            // Assign a unique identifier
            Logger?.WriteLine("Generating identifier...");
            Id = SecureRandom.Integer<ulong>();
        }

        #endregion
    }
}
