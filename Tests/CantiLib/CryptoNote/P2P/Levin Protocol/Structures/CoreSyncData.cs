using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    internal struct CoreSyncData
    {
        internal uint CurrentHeight { get; set; }
        internal string TopId { get; set; }
        public byte[] Serialize()
        {
            PortableStorage Storage = new PortableStorage();
            Storage.AddEntry("current_height", CurrentHeight);
            Storage.AddEntry("top_id", TopId);
            return Storage.Serialize(false);
        }
    }
}
