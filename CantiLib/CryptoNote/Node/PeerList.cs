//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Linq;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        // Contains a list of our connected peers
        internal List<Peer> PeerList { get; set; }

        #endregion

        #region Methods

        // Sets up our peer list
        private void SetupPeerList()
        {
            PeerList = new List<Peer>();
        }

        // Adds a new peer based on a P2P peer object reference
        private Peer AddPeer(P2pPeer P2pPeer)
        {
            lock (PeerList)
            {
                var Peer = new Peer(this, P2pPeer);
                PeerList.Add(Peer);
                return Peer;
            }
        }

        // Removes a peer based on a P2P peer object reference
        private void RemovePeer(P2pPeer P2pPeer)
        {
            lock (PeerList)
            {
                PeerList.RemoveAll(x => x.P2pPeer == P2pPeer);
            }
        }

        // Gets a peer based on a P2P peer object reference
        private Peer GetPeer(P2pPeer P2pPeer)
        {
            lock (PeerList)
            {
                try
                {
                    return PeerList.First(x => x.P2pPeer == P2pPeer);
                }
                catch
                {
                    return AddPeer(P2pPeer);
                }
            }
        }

        // Serializes our peer list as a byte array
        internal byte[] GetPeerList()
        {
            byte[] Buffer = new byte[0];
            foreach (var Peer in PeerList)
            {
                Buffer = Buffer.AppendInteger(Peer.IpAddress);
                Buffer = Buffer.AppendInteger(Peer.Port);
                Buffer = Buffer.AppendInteger(Peer.Id);
                Buffer = Buffer.AppendInteger(Peer.LastSeen);
            }
            return Buffer;
        }

        #endregion
    }
}
