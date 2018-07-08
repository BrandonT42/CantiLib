namespace Canti.CryptoNote.P2P
{
    // Wraps a peer connection with levin specific data
    internal class LevinPeer
    {
        // Internal variables
        internal byte[] Data = new byte[0];
        internal BucketHead2 Header = default(BucketHead2);
        internal PacketReadStatus ReadStatus = PacketReadStatus.Head;
    }

    // Gives names to read status
    internal enum PacketReadStatus : int
    {
        Head = 0,
        Body = 1
    }
}
