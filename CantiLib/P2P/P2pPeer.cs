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
    public sealed class P2pPeer
    {
        #region Properties and Fields

        #region Public

        // Returns whether a peer is still connected or not
        internal bool Connected
        {
            get
            {
                return (Client != null) ? Client.Connected : false;
            }
        }

        // Returns the remote address and port of the peer
        private EndPoint _address;
        internal EndPoint Address
        {
            get
            {
                // Only read and store address from the connection the first time
                if (_address == null && Connected)
                {
                    _address = Client.Client.RemoteEndPoint;
                }

                // Return stored address (so we can still access it after disconnection)
                return _address;
            }
        }

        // The raw TCP client connection
        internal TcpClient Client { get; set; }

        // Specifies the direction this peer connection is coming from
        internal bool IsIncoming { get; set; }

        #endregion

        #region Private

        private P2pServer Server { get; set; }
        private bool StopRunning { get; set; }
        private Thread ReadThread { get; set; }
        private Thread WriteThread { get; set; }
        private Queue<byte[]> OutgoingMessageQueue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Internal

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

        internal void Stop()
        {
            // Stop threads
            StopRunning = true;

            // Stop client
            if (Connected)
            {
                Client.Close();
            }
        }

        #endregion

        #region Public

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

        internal P2pPeer(P2pServer Server, TcpClient Client, bool IsIncoming = true)
        {
            // Assign local variables
            this.Server = Server;
            this.Client = Client;
            this.IsIncoming = IsIncoming;
            StopRunning = false;
            OutgoingMessageQueue = new Queue<byte[]>();
        }

        #endregion
    }
}
