//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Canti
{
    public static class Utils
    {
        #region Encoding

        #region Integers
        public static byte[] IntegerToByteArray(dynamic Input)
        {
            byte[] Output = new byte[0];
            Type ValueType = Input.GetType();
            if (ValueType == typeof(byte) || ValueType == typeof(sbyte)) Output = new byte[1];
            else if (ValueType == typeof(ushort) || ValueType == typeof(short)) Output = new byte[2];
            else if (ValueType == typeof(uint) || ValueType == typeof(int)) Output = new byte[4];
            else if (ValueType == typeof(ulong) || ValueType == typeof(long)) Output = new byte[8];

            for (int i = 0; i < Output.Length; i++)
                Output[i] = (byte)((Convert.ToUInt64(Input) >> i * 8) & 0xFF);
            return Output;
        }

        public static T ByteArrayToInteger<T>(byte[] Input) where T : IComparable<T>
        {
            int Offset = Marshal.SizeOf(default(T)) - 1;
            ulong Output = ((ulong)Input[Offset] << Offset * 8);
            for (; Offset >= 0; Offset--)
                Output |= ((ulong)Input[Offset] << Offset * 8);
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        public static T ByteArrayToInteger<T>(byte[] Input, int Offset) where T : IComparable<T>
        {
            byte[] Bytes = new byte[Marshal.SizeOf(default(T))];
            Buffer.BlockCopy(Input, Offset, Bytes, 0, Bytes.Length);
            return ByteArrayToInteger<T>(Bytes);
        }

        #endregion

        #region VarInts

        public static byte[] PackP2pVarInt(dynamic Input)
        {
            if (Input <= byte.MaxValue) return IntegerToByteArray((byte)(Input << 2 | 0));
            else if (Input <= ushort.MaxValue) return IntegerToByteArray((ushort)(Input << 2 | 1));
            else if (Input <= uint.MaxValue) return IntegerToByteArray((uint)(Input << 2 | 3));
            else if (Input <= ulong.MaxValue) return IntegerToByteArray((ulong)(Input << 2 | 7));
            else return new byte[0];
        }

        public static T UnpackP2pVarInt<T>(byte[] Data, int Offset, out int NewOffset) where T : IComparable<T>
        {
            // Get integer size from mask
            int SizeMask = Data[Offset] & 0x03;
            int BytesLeft = 0;

            // Set number of bit shift iterations
            if (SizeMask == 0) BytesLeft = 0;
            else if (SizeMask == 1) BytesLeft = 1;
            else if (SizeMask == 2) BytesLeft = 3;
            else if (SizeMask == 3) BytesLeft = 7;

            // Calculate output number
            ulong Output = Data[Offset];
            for (NewOffset = Offset; NewOffset <= Offset + BytesLeft; NewOffset++)
                Output |= (ulong)Data[NewOffset] << ((NewOffset - Offset) * 8);
            Output >>= 2;

            // Return result
            return (T)Convert.ChangeType(Output, typeof(T));
        }

        #endregion

        #endregion

        #region Conversion

        public static byte[] HexStringToByteArray(string Input)
        {
            byte[] Output = new byte[Input.Length / 2];
            for (int i = 0; i < Input.Length; i += 2)
                Output[i / 2] = Convert.ToByte(Input.Substring(i, 2), 16);
            return Output;
        }

        public static string ByteArrayToHexString(byte[] Input)
        {
            StringBuilder Hex = new StringBuilder(Input.Length * 2);
            foreach (byte Byte in Input) Hex.AppendFormat("{0:x2}", Byte);
            return Hex.ToString();
        }

        #endregion

        #region Byte Array Extensions

        public static byte[] AppendBytes(this byte[] Destination, byte[] Input)
        {
            byte[] Output = new byte[Input.LongLength + Destination.LongLength];
            Buffer.BlockCopy(Destination, 0, Output, 0, Destination.Length);
            Buffer.BlockCopy(Input, 0, Output, Destination.Length, Input.Length);
            return Output;
        }

        public static byte[] AppendInteger<T>(this byte[] Destination, T Input) where T : IComparable<T>
        {
            byte[] tmp = IntegerToByteArray(Input);
            return Destination.AppendBytes(tmp);
        }

        public static byte[] AppendString(this byte[] Destination, string Input)
        {
            byte[] tmp = System.Text.Encoding.UTF8.GetBytes(Input);
            return Destination.AppendBytes(tmp);
        }

        public static byte[] SubBytes(this byte[] Source, int Offset, int Length)
        {
            byte[] Output = new byte[Length];
            Buffer.BlockCopy(Source, Offset, Output, 0, Length);
            return Output;
        }

        public static bool Matches(this byte[] Self, ReadOnlySpan<byte> Value)
        {
            ReadOnlySpan<byte> Tmp = Self;
            return Tmp.SequenceEqual(Value);
        }

        #endregion

        #region Miscellaneous

        public static ulong GetTimestamp()
        {
            return (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        #endregion
    }
}
