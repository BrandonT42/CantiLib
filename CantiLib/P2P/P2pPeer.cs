//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Canti
{
    /// <summary>
    /// A P2P peer belonging to a P2pServer
    /// </summary>
    public sealed class P2pPeer
    {
        #region Properties and Fields

        #region Internal

        // Returns whether or not this peer's last request indicated an active connection
        internal bool Connected
        {
            get
            {
                // TODO - Poll for connection here?
                return (Client != null) ? Client.Connected : false;
            }
        }

        // Returns the remote address and port of the peer
        internal EndPoint Address { get; private set; }

        // The raw TCP client connection
        internal TcpClient Client { get; set; }

        // Specifies the direction this peer connection is coming from
        internal bool IsIncoming { get; set; }

        #endregion

        #region Private

        // The P2P server this peer is associated with
        private P2pServer Server { get; set; }

        // Signals that this peer should stop its threads
        private bool StopRunning { get; set; }

        // Thread that handles reading incoming data
        private Thread ReadThread { get; set; }

        // Thread that handles sending outgoing data from the queue
        private Thread WriteThread { get; set; }

        // Keeps track of outgoing data arrays
        private Queue<byte[]> OutgoingMessageQueue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal

        // Starts this peer's associated threads
        internal void Start()
        {
            // Begin threads
            ReadThread = new Thread(Read);
            WriteThread = new Thread(Write);
            ReadThread.Start();
            WriteThread.Start();

            // Invoke peer connected event
            Server.OnPeerConnected?.Invoke(this, EventArgs.Empty);
        }

        // Stops this peer's associated threads
        internal void Stop()
        {
            // Stop threads
            StopRunning = true;

            // Stop client
            if (Connected)
            {
                Client.Close();
            }

            // TODO - invoke disconnect here to keep in line with above invoke on connect?
        }

        #endregion

        #region Internal

        // Adds an outgoing message to our queue
        internal void SendMessage(byte[] Data)
        {
            lock (OutgoingMessageQueue)
            {
                OutgoingMessageQueue.Enqueue(Data);
            }
        }

        #endregion

        #region Private

        // Reads incoming data from the client's network stream
        private void Read()
        {
            // Get client stream
            var Stream = Client.GetStream();

            // Create a wait handle array so we can cancel this thread if need be
            while (!StopRunning)
            {
                // Check if we have data that is waiting to be received
                if (Stream.CanRead && Stream.DataAvailable)
                {
                    // Create an empty buffer
                    var Buffer = new byte[Client.Available];

                    // Read to buffer
                    if (Stream.Read(Buffer, 0, Buffer.Length) > 0)
                    {
                        // Invoke data received handler
                        Server.OnDataReceived?.Invoke((this, Buffer), EventArgs.Empty);
                    }
                }

                // TODO - BeginRead
                Thread.Sleep(100);
            }
        }

        // Sends outgoing data from the outgoing data queue
        private void Write()
        {
            // Get client stream
            var Stream = Client.GetStream();

            // Create a wait handle array so we can cancel this thread if need be
            while (!StopRunning)
            {
                // Lock our outgoing message queue to prevent race conditions
                lock (OutgoingMessageQueue)
                {
                    // Check if we have data to send
                    if (OutgoingMessageQueue.Count > 0)
                    {
                        // Get the next packet in line
                        var Data = OutgoingMessageQueue.Dequeue();

                        // Send data to our associated peer
                        Stream.Write(Data, 0, Data.Length);

                        // Invoke data sent handler
                        Server.OnDataSent?.Invoke((this, Data), EventArgs.Empty);
                    }
                }

                // TODO - BeginWrite
                Thread.Sleep(100);
            }
        }

        #endregion

        #endregion

        #region Constructors

        // Initializes a new P2P peer
        internal P2pPeer(P2pServer Server, TcpClient Client, bool IsIncoming = true)
        {
            // Assign local variables
            this.Server = Server;
            this.Client = Client;
            this.IsIncoming = IsIncoming;
            Address = Client.Client.RemoteEndPoint;
            StopRunning = false;
            OutgoingMessageQueue = new Queue<byte[]>();
        }

        #endregion
    }
}
