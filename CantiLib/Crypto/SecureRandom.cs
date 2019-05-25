//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static Canti.Utils;

namespace Canti
{
    // Cryptographically secure random number generation
    public static class SecureRandom
    {
        // Cryptography service provider
        private readonly static RNGCryptoServiceProvider Provider = new RNGCryptoServiceProvider();

        // Generate a random number of a given type
        public static T Integer<T>() where T : IConvertible
        {
            // Get size of output type
            int Size = Marshal.SizeOf(default(T));

            // Create a byte buffer of the given size
            byte[] Bytes = new byte[Size];

            // Generate random bytes usin the cryptography provider
            Provider.GetBytes(Bytes, 0, Size);

            // Convert the output value into a 64 bit integer
            ulong Output = ByteArrayToInteger<ulong>(Bytes);

            // Convert 64 bit integer into the given integer type
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        // Generate a random byte array of a given length
        public static byte[] Bytes(int Length = 0)
        {
            // Create an output array
            byte[] Output = new byte[Length];

            // Populate output array with random bytes
            Provider.GetBytes(Output, 0, Length);

            // Return output
            return Output;
        }

        // Generate a random string of letters and numbers
        public static string String(int Length = 64)
        {
            // Create a buffer
            byte[] Buffer = new byte[0];

            // Loop through and add random bytes until the string is of the desired length
            for (int i = 0; i < Length / 2; i++)
                Buffer = Buffer.AppendInteger(Integer<byte>());

            // Convert to a hex string
            string Output = ByteArrayToHexString(Buffer);

            // Trim to correct size
            while (Output.Length < Length) Output += Integer<byte>();
            if (Output.Length > Length) Output = Output.Substring(0, Length);

            // Return output string
            return Output;
        }
    }
}
