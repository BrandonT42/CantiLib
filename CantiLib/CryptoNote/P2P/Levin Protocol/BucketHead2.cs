using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    // Header for levin protocol requests
    [Serializable]
    internal struct bucket_head2
    {
        internal ulong m_signature { get; set; }
        internal ulong m_cb { get; set; }
        internal bool m_have_to_return_data { get; set; }
        internal uint m_command { get; set; }
        internal int m_return_code { get; set; }
        internal uint m_flags { get; set; }
        internal uint m_protocol_version { get; set; }
    };
}
