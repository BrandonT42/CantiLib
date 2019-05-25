//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    internal sealed class PacketHeader
    {
        #region Properties

        // This signature number must match on both the receiving and sending sides
        public ulong Signature { get; set; }

        // This is the version of the levin protocol we will be using
        public uint ProtocolVersion { get; set; }

        // Command code is the packet type
        public uint CommandCode { get; set; }

        // Whether or not we are requesting a response
        public bool ResponseRequired { get; set; }

        // Payload size is how many bytes the body of this packet contains
        public ulong PayloadSize { get; set; }

        // Return code that says whether or not a request was successful or not
        public uint ReturnCode { get; set; }

        // Flag is the packet type (request or response)
        public uint Flag { get; set; }

        #endregion

        #region Methods

        // Serializes the header information into a byte array
        // For reference - A levin packet header is always 33 bytes,
        // and has data written in the following order:
        // Signature (8 bytes)
        // Payload Size (8 bytes)
        // Response Required (1 byte)
        // Command Code (4 bytes)
        // Return Code (4 bytes)
        // Flag (4 bytes)
        // Protocol Version (4 bytes)
        public byte[] Serialize()
        {
            // Create output buffer
            byte[] Output = new byte[0];

            // Add our packet header information to our output buffer
            Output = Output.AppendBytes(IntegerToByteArray(Signature));
            Output = Output.AppendBytes(IntegerToByteArray(PayloadSize));
            Output = Output.AppendBytes(new byte[] { Convert.ToByte(ResponseRequired) });
            Output = Output.AppendBytes(IntegerToByteArray(CommandCode));
            Output = Output.AppendBytes(IntegerToByteArray(ReturnCode));
            Output = Output.AppendBytes(IntegerToByteArray(Flag));
            Output = Output.AppendBytes(IntegerToByteArray(ProtocolVersion));

            // Return output buffer
            return Output;
        }

        #endregion

        #region Constructors

        // Deserializes the header information from a byte array
        public PacketHeader(byte[] Data)
        {
            // Decode signature
            Signature = ByteArrayToInteger<ulong>(Data);
            Data = Data.SubBytes(8, Data.Length - 8);

            // Decode payload size
            PayloadSize = ByteArrayToInteger<ulong>(Data);
            Data = Data.SubBytes(8, Data.Length - 8);

            // Decode response required
            ResponseRequired = Convert.ToBoolean(Data[0]);
            Data = Data.SubBytes(1, Data.Length - 1);

            // Decode command code
            CommandCode = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode return code
            ReturnCode = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode flags
            Flag = ByteArrayToInteger<uint>(Data);
            Data = Data.SubBytes(4, Data.Length - 4);

            // Decode protocol version
            ProtocolVersion = ByteArrayToInteger<uint>(Data);
        }

        public PacketHeader() { }

        #endregion
    }
}
