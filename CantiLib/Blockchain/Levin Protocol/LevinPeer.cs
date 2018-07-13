using Canti.Blockchain.P2P;

namespace Canti.Blockchain
{
    // Wraps a peer connection with levin specific data
    internal class LevinPeer
    {
        // Internal variables
        internal byte[] Data = new byte[0];
        internal BucketHead2 Header = default(BucketHead2);
        internal PeerConnection Connection;

        // Peer stats
        internal PacketReadStatus ReadStatus = PacketReadStatus.Head;
        internal PeerState State = PeerState.Unverified;
        // Height
        // ID?
        // Hopes and dreams

        // Entry point
        internal LevinPeer(PeerConnection Connection)
        {
            this.Connection = Connection;
        }
    }

    // Gives names to read status
    internal enum PacketReadStatus : int
    {
        Head = 0,
        Body = 1
    }

    // A peer's connection state
    internal enum PeerState : int
    {
        Unverified = 0,
        Verified = 1
    }
}
