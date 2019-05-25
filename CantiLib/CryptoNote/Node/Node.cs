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

        // This node's blockchain storage handler
        private BlockchainStorage Blockchain { get; set; }

        // This node's P2P server
        private P2pServer P2pServer { get; set; }

        // This node's API server
        private ApiServer ApiServer { get; set; }

        // This node's unique ID
        internal ulong Id { get; set; }

        // The port our P2P server will bind to
        public int P2pPort = Globals.P2P_DEFAULT_PORT;

        // The port our API server will bind to
        public int ApiPort = Globals.API_DEFAULT_PORT;

        #endregion

        #region Methods

        // Starts our node and any required threads
        public bool Start()
        {
            // Start P2P server
            Logger.WriteLine("Starting P2P server...");
            try { P2pServer.Start(P2pPort); }
            catch (Exception e)
            {
                Logger.Error($"Could not start P2P server: {e.Message}");
                return false;
            }
            Logger.WriteLine($"P2P server started on port {P2pServer.Port}");

            // Start API server
            Logger.WriteLine("Starting API server...");
            try { ApiServer.Start(ApiPort); }
            catch (Exception e)
            {
                Logger.Error($"Could not start API server: {e.Message}");
                return false;
            }
            Logger.WriteLine($"API server started on port {ApiServer.Port}");

            // Start sync process
            Logger.WriteLine("Starting sync process...");
            StartSync();

            // Start is completed
            Logger.Important($"Node initialized with ID {Id}");
            return true;
        }

        // Stops our node and ends all threads in a safe manner
        public void Stop()
        {
            // Stop API server
            Logger.WriteLine("Stopping API server...");
            ApiServer.Stop();

            // Stop P2P server
            Logger.WriteLine("Stopping P2P server...");
            P2pServer.Stop();

            // Dispose of all connected peers
            Logger.WriteLine("Disposing of peers...");
            foreach (var Peer in PeerList)
            {
                Peer.Dispose();
            }
            Logger.Important("Node stopped.");
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

        public Node()
        {
            // Setup our blockchain handler
            Logger.WriteLine("Setting up blockchain handler...");
            Blockchain = new BlockchainStorage();

            // Create our P2P server
            Logger.WriteLine("Setting up P2P server...");
            P2pServer = new P2pServer(Globals.P2P_MAX_PEER_CONNECTIONS);

            // Assign P2P event handlers
            Logger.WriteLine("Assigning P2P callbacks...");
            AssignCallbacks();

            // Setup our API server
            Logger.WriteLine("Setting up API server...");
            ApiServer = new ApiServer(Globals.API_MAX_WORKERS);

            // Create an API context
            Logger.WriteLine("Assigning API method context...");
            ApiServer.AssignMethodContext(new ApiMethods(this));

            // Setup our peer list
            Logger.WriteLine("Setting up local peer list...");
            SetupPeerList();

            // Assign a unique identifier
            Logger.WriteLine("Generating identifier...");
            Id = SecureRandom.Integer<ulong>();
        }

        #endregion
    }
}
