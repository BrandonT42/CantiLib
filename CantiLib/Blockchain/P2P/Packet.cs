//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain.P2P
{
    internal struct Packet
    {
        internal PeerConnection Peer;
        internal byte[] Data;
        internal Packet(PeerConnection Peer, byte[] Data)
        {
            this.Peer = Peer;
            this.Data = Data;
        }
    }
}
