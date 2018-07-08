using Canti.Data;
using System;

namespace Canti.CryptoNote.P2P
{
    // Header for levin protocol requests
    [Serializable]
    internal struct BucketHead2
    {
        // Header variables (names should be self explanatory)
        internal ulong Signature { get; set; }
        internal ulong PayloadSize { get; set; }
        internal bool ResponseRequired { get; set; }
        internal uint CommandCode { get; set; }
        internal int ReturnCode { get; set; }
        internal uint Flags { get; set; }
        internal uint ProtocolVersion { get; set; }

        // Serializes the header information into a byte array
        internal byte[] Serialize()
        {
            // Create output array
            byte[] Output = new byte[33];
            int Offset = 0;

            // Add signature bytes
            byte[] CurrentBytes = Encoding.UlongToByteArray(Signature);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add payload size bytes
            CurrentBytes = Encoding.UlongToByteArray(PayloadSize);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add response required bytes
            CurrentBytes = new byte[] { ResponseRequired ? (byte)1 : (byte)0 };
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add command code bytes
            CurrentBytes = Encoding.UlongToByteArray(CommandCode);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add return code bytes
            CurrentBytes = Encoding.IntToByteArray(ReturnCode);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add flags bytes
            CurrentBytes = Encoding.UlongToByteArray(Flags);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Add protocol version bytes
            CurrentBytes = Encoding.UlongToByteArray(ProtocolVersion);
            Buffer.BlockCopy(CurrentBytes, 0, Output, Offset, CurrentBytes.Length);
            Offset += CurrentBytes.Length;

            // Return output array
            return Output;
        }
    };
}
