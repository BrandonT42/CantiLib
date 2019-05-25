//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    // A node, no matter the protocol, should have these methods implemented
    // Interfacing like this allows future extensibility in regards to future protocols
    public interface INode
    {
        #region Methods

        // Reference to console logger
        Logger Logger { get; set; }

        // Starts this node
        bool Start();

        // Stops this node
        void Stop();

        // Manually adds a peer to this node
        void AddPeer(string Address, int Port);

        // Forcefully adds a peer to this node
        void ForceAddPeer(string Address, int Port);

        #region P2P Server Callbacks

        void OnStart(object sender, EventArgs e);
        void OnStop(object sender, EventArgs e);
        void OnP2pPeerConnected(object sender, EventArgs e);
        void OnP2pPeerDisconnected(object sender, EventArgs e);
        void OnDataReceived(object sender, EventArgs e);
        void OnDataSent(object sender, EventArgs e);

        #endregion

        #endregion
    }
}
