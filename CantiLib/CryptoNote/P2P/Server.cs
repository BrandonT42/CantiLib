using Canti.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

// TODO:
// - Handshake
// - Prune old peers, check for disconnect
// - Add OnCommandSent and OnDataSent events
// - Fix Close not actually closing everything... Find where it's hanging up
namespace Canti.CryptoNote.P2P
{
    public class Server
    {
        // Private variables
        private TcpListener Listener;
        private List<PeerConnection> Peers = new List<PeerConnection>();
        private Queue<Packet> IncomingRequests = new Queue<Packet>();
        private Queue<Packet> OutgoingRequests = new Queue<Packet>();
        private Thread IncomingRequestThread;
        private Thread OutgoingRequestThread;
        private Thread PeerConnectionThread;

        // Define data handling context
        private LevinProtocol Context;

        // Internal variables
        internal bool Running = true;

        // Event handlers
        public EventHandler OnError;
        public EventHandler OnDataReceived;
        public EventHandler OnDataSent;
        public EventHandler OnPeerConnected;
        // public EventHandler OnPeerDisconnected;

        // Start server on specified port
        public void Start(Int32 Port)
        {
            // Create a new TCP listener and start listening
            Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);
            Listener.Start();

            // Create a levin protocol context
            Context = new LevinProtocol(this);

            // Start request handling thread
            IncomingRequestThread = new Thread(ProcessIncomingRequests);
            IncomingRequestThread.Start();

            // Start request handling thread
            OutgoingRequestThread = new Thread(ProcessOutgoingRequests);
            OutgoingRequestThread.Start();

            // Start connection handling thread
            PeerConnectionThread = new Thread(PeerConnection);
            PeerConnectionThread.Start();
        }

        // Handles all incoming connections
        private void PeerConnection()
        {
            // Begin update loop
            while (Running)
            {
                // Loop as long as there are pending connections
                while (Listener.Pending())
                {
                    // Accept client connection
                    TcpClient Client = Listener.AcceptTcpClient();
                    Console.WriteLine("[Server] Received a peer connection from {0}", Client.Client.RemoteEndPoint);

                    // Add to peer list
                    Peers.Add(new PeerConnection(this, Client));

                    // Invoke connection event handler
                    OnPeerConnected?.Invoke(Client, EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Handles all incoming requests in order of appearance
        private void ProcessIncomingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (IncomingRequests.Count > 0) OnDataReceived?.Invoke(IncomingRequests.Dequeue(), EventArgs.Empty);

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Handles all outgoing requests in order of appearance
        private void ProcessOutgoingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (OutgoingRequests.Count > 0)
                {
                    // Get request data from queue
                    Packet Request = OutgoingRequests.Dequeue();

                    // Broadcast data if no client is specified
                    if (Request.Peer == null) Broadcast(Request.Data);

                    // Send to specified peer
                    else SendMessage(Request.Peer, Request.Data);

                    // Invoke data sent event
                    OnDataSent?.Invoke(Request, EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(200);
            }
        }

        // Connect to a specified URL
        public void Connect(Connection Connection)
        {
            // Try to connect to peer
            try
            {
                // Create a connection
                TcpClient Client = new TcpClient(Connection.Host, Connection.Port);
                Peers.Add(new PeerConnection(this, Client));

                // Invoke connection event handler
                OnPeerConnected?.Invoke(Client, EventArgs.Empty);
            }

            // Unable to connect to peer
            catch
            {
                // Raise error event
                OnError?.Invoke("Unable to connect to peer " + Connection.Host + ":" + Connection.Port, EventArgs.Empty);
            }
        }

        // Broadcast data to all peers
        internal void Broadcast(byte[] Data)
        {
            // Send data to all peers
            foreach (PeerConnection Peer in Peers) Peer.SendMessage(Data);
        }

        // Send data to a specified peer
        internal void SendMessage(PeerConnection Peer, byte[] Data)
        {
            // Send data to peer
            Peer.SendMessage(Data);
        }

        // Closes all connections
        public void Close()
        {
            // Set running status to false
            Running = false;

            // Stop the listener
            Listener.Stop();

            // Clear all collections
            IncomingRequests.Clear();
            OutgoingRequests.Clear();
            Peers.Clear();
        }
    }
}
