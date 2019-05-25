//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    public sealed partial class Node : INode
    {
        #region Properties and Fields

        // Holds configuration for everything on the network
        internal NetworkConfig Globals { get; set; }

        // This node's blockchain storage handler
        private BlockchainStorage Blockchain { get; set; }

        // This node's P2P server
        private P2pServer P2pServer { get; set; }

        // This node's API server
        private ApiServer ApiServer { get; set; }

        // The logger we will use to output to
        public Logger Logger { get; set; }

        // This node's unique ID
        internal ulong Id { get; set; }

        // The port our P2P server will bind to
        public int P2pPort { get; set; }

        // The port our API server will bind to
        public int ApiPort { get; set; }

        #endregion

        #region Methods

        // Starts our node and any required threads
        public bool Start()
        {
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

        // Stops our node and ends all threads in a safe manner
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

        // Manually adds a peer to our P2P connection queue
        public void AddPeer(string Address, int Port)
        {
            // Add peer to server
            P2pServer.AddPeer(Address, Port);
        }

        // Manually adds a peer to our P2P peer list, replacing one if needed
        public void ForceAddPeer(string Address, int Port)
        {
            // Force-add peer to server
            P2pServer.ForceAddPeer(Address, Port);
        }

        #endregion

        #region Constructors

        public Node(NetworkConfig Configuration)
        {
            // Assign configuration
            Globals = Configuration;

            // Setup logger
            Logger = new Logger()
            {
                LogFile = Globals.LOG_FILE,
                LogLevel = Globals.LOG_LEVEL,
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
