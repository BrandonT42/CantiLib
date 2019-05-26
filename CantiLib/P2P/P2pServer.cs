//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Canti
{
    /// <summary>
    /// A standalone P2P server
    /// </summary>
    public sealed class P2pServer
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// Invoked when the server is started successfully
        /// </summary>
        /// <param name="sender">A reference to this P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnStart { get; set; }

        /// <summary>
        /// Invoked when the server is stopped
        /// </summary>
        /// <param name="sender">A reference to this P2pServer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnStop { get; set; }

        /// <summary>
        /// Invoked when a new peer connection is detected
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnPeerConnected { get; set; }

        /// <summary>
        /// Invoked when a peer's polling attempt comes back unsuccessful, indicating a disconnection
        /// </summary>
        /// <param name="sender">A reference to the associated P2pPeer instance</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnPeerDisconnected { get; set; }

        /// <summary>
        /// Invoked when a connected peer sends data
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data received</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnDataReceived { get; set; }

        /// <summary>
        /// Invoked when the server sends data to a connected peer
        /// </summary>
        /// <param name="sender">(P2pPeer Peer, byte[] Data) - A reference to 
        /// the associated P2pPeer instance, as well as the data sent</param>
        /// <param name="e">Always empty</param>
        public EventHandler OnDataSent { get; set; }

        /// <summary>
        /// The maximum number of concurrent peer connections this server will allow
        /// </summary>
        public int MaxConnections { get; private set; }

        /// <summary>
        /// The port this server is binded to
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// How many peers are connected to this server
        /// </summary>
        public int Connections
        {
            get
            {
                lock (Peers)
                {
                    return Peers.Values.Where(x => x != null && x.Connected).Count();
                }
            }
        }

        /// <summary>
        /// Returns a list of all connected peers
        /// </summary>
        public List<P2pPeer> PeerList
        {
            get
            {
                lock (Peers)
                {
                    return Peers.Values.Where(x => x != null && x.Connected).ToList();
                }
            }
        }

        #endregion

        #region Private

        // Our listener
        private TcpListener Listener { get; set; }

        // The thread our listener runs on
        private Thread ListenerThread { get; set; }

        // Event that is set when the server is stopped
        private ManualResetEvent StopEvent { get; set; }

        // Event that is set when the server detects a pending connection
        private ManualResetEvent ReadyEvent { get; set; }

        // Queue of incoming connections
        private Queue<(TcpClient Client, bool IsIncoming)> ConnectionQueue { get; set; }

        // Dictionary that links one peer worker thread to a peer client
        private Dictionary<Thread, P2pPeer> Peers { get; set; }

        // Handles connection polling
        private Timer PollingTimer { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts listening for new connections
        /// </summary>
        /// <param name="Port">The port to listen for incoming connections on</param>
        /// <param name="PollingInterval">How often (in milliseconds) connections should be polled</param>
        public void Start(int Port, int PollingInterval)
        {
            // Store port
            this.Port = Port;

            // Setup and start our TCP listener on our desired port
            var EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            Listener = new TcpListener(EndPoint);

            // Start our listener and its associated thread
            Listener.Start();
            ListenerThread.Start();

            // Assign and start each of our peer worker threads
            foreach (var Worker in Peers.Keys)
            {
                Worker.Start();
            }

            // Begin polling connections
            PollingTimer = new Timer(new TimerCallback(Poll), null, 0, PollingInterval);

            // Invoke on start event handler, signalling that our threads have begun
            OnStart?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Stops the server and ends all associated threads
        /// </summary>
        public void Stop()
        {
            // Stop polling timer
            PollingTimer.Dispose();

            // Sets a stop event, signalling we want threads to exit
            StopEvent.Set();

            // Join our main thread and each peer thread, passing along this signal
            ListenerThread.Join();
            foreach (var Peer in Peers)
            {
                Peer.Key.Join();
                Peer.Value?.Stop();
            }

            // Stop our TCP listener
            Listener.Stop();

            // Invoke on stop event handler, signalling that our threads have exited
            OnStop?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Broadcasts a byte array to all connected peers
        /// </summary>
        /// <param name="Data">The byte array to be sent</param>
        public void Broadcast(byte[] Data)
        {
            // Send our data to each connection
            foreach (var Peer in Peers.Where(x => x.Value != null))
            {
                Peer.Value.SendMessage(Data);
            }
        }

        /// <summary>
        /// Adds a peer to the connection queue to be accepted when space is available
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        public void AddPeer(string Address, int Port)
        {
            // Do this in a new thread to prevent locking on the main thread while connecting
            new Thread(delegate ()
            {
                // TODO - better error feedback
                try
                {
                    // Create a new TcpClient instance
                    var Peer = new TcpClient(Address, Port);

                    // Lock our connection queue to prevent any race conditions
                    lock (ConnectionQueue)
                    {
                        // Add our peer to the queue
                        ConnectionQueue.Enqueue((Peer, false));

                        // Signal a ready event
                        ReadyEvent.Set();
                    }
                }
                catch { }
            }).Start();
        }

        /// <summary>
        /// Adds a peer, ignoring the current connection queue, and booting another peer if needed
        /// </summary>
        /// <param name="Address">The remote peer's host address</param>
        /// <param name="Port">The remote peer's listening port</param>
        public void ForceAddPeer(string Address, int Port)
        {
            // Do this in a new thread to prevent locking on the main thread while connecting
            new Thread(delegate ()
            {
                // TODO - better error handling
                try
                {
                    // Lock our connection queue to prevent any race conditions
                    lock (ConnectionQueue)
                    {
                        // Connect with a new TcpClient instance
                        var Peer = new TcpClient(Address, Port);

                        // Get the first peer index
                        // TODO - round-robin select this in the future
                        var tmp = Peers.First();

                        // Stop this peer if it's already running
                        if (tmp.Value != null && tmp.Value.Connected)
                        {
                            tmp.Value.Stop();
                        }

                        // Add this peer to our list
                        AddPeer(Peer, false, tmp.Key);
                    }
                }
                catch { }
            }).Start();
        }

        #endregion

        #region Private

        // Listens for new connections and enqueues them
        private void Listen()
        {
            while (Listener.Server.IsBound)
            {
                // Accept incoming connections
                var Client = Listener.BeginAcceptTcpClient((IAsyncResult Result) =>
                {
                    try
                    {
                        // Lock our connection queue to prevent any race conditions
                        lock (ConnectionQueue)
                        {
                            // Add the incoming connection to our connection queue
                            ConnectionQueue.Enqueue((Listener.EndAcceptTcpClient(Result), true));

                            // Signal that a connection is ready to be accepted
                            ReadyEvent.Set();
                        }
                    }
                    catch { }
                }, null);

                // Wait for exit
                if (WaitHandle.WaitAny(new[] { StopEvent, Client.AsyncWaitHandle }) == 0) return;
            }
        }

        // Accepts incoming connections if we are able
        private void AcceptPeerConnection()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(wait))
            {
                // Lock our connection queue to prevent any race conditions
                lock (ConnectionQueue)
                {
                    // Connection queue has entries, accept one
                    if (ConnectionQueue.Count > 0)
                    {
                        // Dequeue the new peer in line
                        var Peer = ConnectionQueue.Dequeue();

                        // Handle this connection
                        AddPeer(Peer.Client, Peer.IsIncoming);
                    }

                    // There are no entries in the connection queue
                    else
                    {
                        // No peers in line, reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        // Polls peer connections to see if any have disconnected
        private void Poll(object _)
        {
            // Iterate over all connected peers
            foreach (var Peer in PeerList)
            {
                // Send a polling request
                if (Peer.Client.Client.Poll(0, SelectMode.SelectRead))
                {
                    try
                    {
                        // Receive response
                        if (Peer.Client.Client.Receive(new byte[1], SocketFlags.Peek) == 0)
                        {
                            // Remove peer from list
                            RemovePeer(Peer);
                        }
                    }
                    catch
                    {
                        // Remove peer from list
                        RemovePeer(Peer);
                    }
                }
            }
        }

        // Handles a new peer client
        private void AddPeer(TcpClient PeerClient, bool IsIncoming = true, Thread Thread = null)
        {
            // Default to current thread if not manually set
            Thread Key = Thread ?? Thread.CurrentThread;

            // Add to connection dictionary
            Peers[Key] = new P2pPeer(this, PeerClient, IsIncoming);
            Peers[Key].Start();
        }

        // Removes a peer from the peer list
        private void RemovePeer(P2pPeer Peer)
        {
            // Stop this peer
            Peer.Stop();

            // Remove this peer from our peer list
            Peers[Peers.First(x => x.Value != null && x.Value.Address == Peer.Address).Key] = null;

            // Invoke peer disconnected event
            OnPeerDisconnected?.Invoke(Peer, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new P2P server
        /// </summary>
        /// <param name="MaxConnections">The maximum number of concurrent peer connections this server will allow</param>
        public P2pServer(int MaxConnections)
        {
            // Setup threads and events
            Peers = new Dictionary<Thread, P2pPeer>();
            for (int i = 0; i < MaxConnections; i++)
            {
                Thread Worker = new Thread(AcceptPeerConnection);
                Peers.Add(Worker, null);
            }
            ListenerThread = new Thread(Listen);
            ReadyEvent = new ManualResetEvent(false);
            StopEvent = new ManualResetEvent(false);
            ConnectionQueue = new Queue<(TcpClient Client, bool IsIncoming)>();
        }

        #endregion
    }
}
