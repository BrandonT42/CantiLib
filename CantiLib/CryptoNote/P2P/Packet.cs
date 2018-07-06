namespace Canti.CryptoNote.P2P
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
