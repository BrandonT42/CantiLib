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
    /// <summary>
    /// Cryptographically secure random number generation
    /// </summary>
    public static class SecureRandom
    {
        #region Properties and Fields

        #region Private

        // Cryptography service provider
        private readonly static RNGCryptoServiceProvider Provider = new RNGCryptoServiceProvider();

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Generate a random number of a given type
        /// </summary>
        /// <typeparam name="T">The type of integer to generate</typeparam>
        /// <returns>A random integer of the specified type</returns>
        public static T Integer<T>() where T : IComparable<T>
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

        /// <summary>
        /// Generate a random byte array of a given length
        /// </summary>
        /// <param name="Length">How long the generate byte array should be</param>
        /// <returns>A randomized byte array of the specified length</returns>
        public static byte[] Bytes(int Length = 0)
        {
            // Create an output array
            byte[] Output = new byte[Length];

            // Populate output array with random bytes
            Provider.GetBytes(Output, 0, Length);

            // Return output
            return Output;
        }

        #endregion
    }
}
