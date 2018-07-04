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
        // Variables
        private TcpListener Listener;
        private IDictionary<TcpClient, Thread> Peers = new Dictionary<TcpClient, Thread>();
        private IDictionary<Connection, TcpClient> ConnectedPeers = new Dictionary<Connection, TcpClient>();
        private Queue<(TcpClient Peer, byte[] Data)> IncomingRequests = new Queue<(TcpClient Peer, byte[] Data)>();
        internal Queue<P2pMessage> Broadcasts = new Queue<P2pMessage>();
        private Thread IncomingRequestThread;
        private Thread OutgoingRequestThread;
        private LevinProtocol Context;
        private bool Running = true;

        // Event handlers
        public EventHandler OnDataReceived;
        //public EventHandler OnDataSent;
        public EventHandler OnCommandReceived;
        public EventHandler OnRequestSent;
        public EventHandler OnPeerConnected;
        // public EventHandler OnPeerDisconnected;
        public EventHandler OnError;

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

            // Start connection loop
            new Thread(delegate ()
            {
                while (Running)
                {
                    // Loop as long as there are pending connections
                    while (Listener.Pending())
                    {
                        // Accept client connection
                        TcpClient Client = Listener.AcceptTcpClient();
                        Console.WriteLine("[Server] Received a peer connection from {0}", Client.Client.RemoteEndPoint);

                        // Add to peer list and start peer thread
                        Peers.Add(Client, new Thread(() => PeerConnection(Client)));
                        Peers[Client].Start();

                        // Invoke connection event handler
                        OnPeerConnected?.Invoke(Client, EventArgs.Empty);
                    }

                    // Let the thread sleep
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        // Handles peer connections
        private void PeerConnection(TcpClient Client)
        {
            NetworkStream PeerStream = Client.GetStream();

            // Begin an update loop
            while (Running)
            {
                // Check if data is available
                while (PeerStream.DataAvailable)
                {
                    // Create a byte buffer then read incoming data
                    Byte[] Buffer = new Byte[Client.Available];
                    PeerStream.Read(Buffer, 0, Buffer.Length);
                    // ^-- May need to find a way to wait to know whether a request has been fully received yet or not?
                    IncomingRequests.Enqueue((Client, Buffer));
                }

                // Let the thread sleep
                Thread.Sleep(1000);
            }

            // Close connection
            Client.Close();
        }

        // Handles all incoming requests in order of appearance
        private void ProcessIncomingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (IncomingRequests.Count > 0)
                {
                    // Get request data from queue
                    (TcpClient Peer, byte[] Data) Request = IncomingRequests.Dequeue();

                    // Invoke data received event
                    OnDataReceived?.Invoke(new Packet(Request.Peer, Request.Data), EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(1000);
            }
        }

        // Handles all outgoing requests in order of appearance
        private void ProcessOutgoingRequests()
        {
            while (Running)
            {
                // Wait for requests
                while (Broadcasts.Count > 0)
                {
                    // Get request data from queue
                    P2pMessage Request = Broadcasts.Dequeue();

                    // Broadcast message
                    Broadcast(Request.Type, Request.Data, false);

                    // Invoke data received event
                    OnRequestSent?.Invoke(Request, EventArgs.Empty);
                }

                // Let the thread sleep
                Thread.Sleep(1000);
            }
        }

        // Connect to a specified URL
        public void Connect(Connection Connection)
        {
            // Check if peer has already been added
            if (!ConnectedPeers.ContainsKey(Connection))
            {
                // Create a connection
                TcpClient Client = new TcpClient(Connection.Host, Connection.Port);
                ConnectedPeers.Add(Connection, Client);
                Peers.Add(Client, new Thread(() => PeerConnection(Client)));
                Peers[Client].Start();

                // Send a handshake
                Commands.Handshake.Request req = new Commands.Handshake.Request
                {
                    //req.NodeData = m_node.getNodeData();
                    PayloadData = new CORE_SYNC_DATA()
                };
                Context.SendReply(Client.GetStream(), Commands.Handshake.Id, LevinProtocol.Encode(req), LevinProtocol.LEVIN_PACKET_RESPONSE);

                // Invoke connection event handler
                OnPeerConnected?.Invoke(Client, EventArgs.Empty);
            }
        }

        // Broadcast data to all peers
        internal void Broadcast(byte[] Data)
        {
            foreach (KeyValuePair<TcpClient, Thread> Peer in Peers)
                Peer.Key.GetStream().Write(Data, 0, Data.Length);
        }
        public void Broadcast(UInt32 Command, byte[] Body, bool ResponseRequired)
        {
            Context.BroadcastMessage(Command, Body, ResponseRequired);
        }

        // Closes all connections
        public void Close()
        {
            // Set running status to false
            Running = false;

            // Stop the listener
            Listener.Stop();

            // Clear all collections
            ConnectedPeers.Clear();
            Peers.Clear();
            IncomingRequests.Clear();
        }

        

        

        /*void WriteHandshake(P2pMessage Message)
        {
            object coreSync = LevinProtocol.Decode(Message.Data, coreSync);
            Command Command = Context.ReadCommand()

            if (m_context.isIncoming())
            {
                // response
                Commands.Handshake.Response res = new Commands.Handshake.Response();
                res.NodeData = m_node.getNodeData();
                res.PayloadData = coreSync;
                res.LocalPeerList = m_node.getLocalPeerList();
                m_context.writeMessage(makeReply(COMMAND_HANDSHAKE::ID, LevinProtocol::encode(res), LEVIN_PROTOCOL_RETCODE_SUCCESS));
                m_node.tryPing(m_context);
            }
            else
            {
                // request
                Commands.Handshake.Request req = new Commands.Handshake.Request();
                req.NodeData = m_node.getNodeData();
                req.PayloadData = coreSync;
                m_context.writeMessage(makeRequest(COMMAND_HANDSHAKE::ID, LevinProtocol::encode(req)));
            }
        }*/
    }
}
