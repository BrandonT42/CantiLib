using System;

namespace Canti.CryptoNote.P2P
{
    internal partial class LevinProtocol
    {
        // Server connection
        private Server Connection;

        // Entry point
        internal LevinProtocol(Server Connection)
        {
            // Set connection
            this.Connection = Connection;

            // Bind event handlers
            this.Connection.OnDataReceived += OnDataReceived;
            this.Connection.OnPeerConnected += OnPeerConnected;
        }

        // Data received
        private void OnDataReceived(object sender, EventArgs e)
        {
            // Decode command

            // Process command

            // Send response if requested
        }

        // Peer connected
        private void OnPeerConnected(object sender, EventArgs e)
        {
            // Send ping

            // Await pong

            // Send handhshake
        }
    }
}
