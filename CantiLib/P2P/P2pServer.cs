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
    // A simple multi-threaded P2P wrapper
    public sealed class P2pServer
    {
        #region Properties and Fields

        #region Public

        // This is invoked when all of our threads have started
        public EventHandler OnStart { get; set; }

        // This is invoked when all of our threads have exited
        public EventHandler OnStop { get; set; }

        // This is invoked when a peer connection is formed on a new thread
        public EventHandler OnPeerConnected { get; set; }

        // This is invoked when a peer connection is disconnected
        public EventHandler OnPeerDisconnected { get; set; }

        // This is invoked whenever a peer sends us any data
        public EventHandler OnDataReceived { get; set; }

        // This is invoked whenever a peer is sent data
        public EventHandler OnDataSent { get; set; }

        // The maximum number of connections we will allow at one time for this server
        public int MaxConnections { get; private set; }

        // The port our server was started on
        public int Port { get; private set; }

        // Lists the total active connections
        public int Connections
        {
            get
            {
                return Peers.Values.Where(x => x != null && x.Connected).Count();
            }
        }

        // Returns a referenced list to all active peer clients
        public List<P2pPeer> PeerList
        {
            get
            {
                return Peers.Values.Where(x => x != null && x.Connected).ToList();
            }
        }

        #endregion

        #region Private

        // Our listener
        private TcpListener Listener { get; set; }

        // The thread our listener runs on
        private Thread ListenerThread { get; set; }

        // Thread events
        private ManualResetEvent StopEvent { get; set; }
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

        // Starts our threads and begins listening for connections
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

        // Stops the server and all peer worker threads
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

        // Broadcasts a set of bytes to all connected peer clients
        public void Broadcast(byte[] Data)
        {
            // Send our data to each connection
            foreach (var Peer in Peers.Where(x => x.Value != null))
            {
                Peer.Value.SendMessage(Data);
            }
        }

        // Adds a peer to the connection queue manually (such as adding a seed node)
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

        // Forcefully adds a peer, replacing the first in our list if needed
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

        // Constructor
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
