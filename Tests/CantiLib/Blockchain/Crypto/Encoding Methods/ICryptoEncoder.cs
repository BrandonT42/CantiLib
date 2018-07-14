//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain.Crypto
{
    // Interface for cryptography implementations
    interface ICryptoEncoder
    {
        // Encodes a value or set of values
        string Encode(byte[] Data);
        string EncodeWithCheckSum(byte[] Data);

        // Decodes a value
        byte[] Decode(string Data);
        byte[] DecodeWithCheckSum(string Data);

        // Gets checksum
        byte[] GetCheckSum(byte[] Data);

        // Adds checksum value
        byte[] AddCheckSum(byte[] Data);

        // Verify checksum value
        byte[] VerifyCheckSum(byte[] Data);
    }
}
