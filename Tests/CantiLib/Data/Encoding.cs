using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Canti.Data
{
    public static class Encoding
    {
        #region Objects
        /// <summary>
        /// Encodes an object to a hex string
        /// </summary>
        internal static string ObjectToHexString(object Input)
        {
            byte[] Bytes = ObjectToByteArray(Input);
            string Hex = ByteArrayToHexString(Bytes);
            return Hex;
        }

        /// <summary>
        /// Decodes an object from a hex string
        /// </summary>
        public static object DecodeObject(string Input)
        {
            try
            {
                byte[] Bytes = HexStringToByteArray(Input);
                object Output = DecodeObject(Bytes);
                return Output;
            }
            catch
            {
                return new object();
            }
        }

        /// <summary>
        /// Encodes an object to a byte array
        /// </summary>
        public static byte[] ObjectToByteArray(object Input)
        {
            BinaryFormatter Binary = new BinaryFormatter();
            using (MemoryStream Stream = new MemoryStream())
            {
                Binary.Serialize(Stream, Input);
                return Stream.ToArray();
            }
        }

        /// <summary>
        /// Decodes a byte array to an object
        /// </summary>
        public static object DecodeObject(byte[] Input)
        {
            using (var Stream = new MemoryStream())
            {
                BinaryFormatter Binary = new BinaryFormatter();
                Stream.Write(Input, 0, Input.Length);
                Stream.Seek(0, SeekOrigin.Begin);
                return Binary.Deserialize(Stream);
            }
        }

        public static byte[] StringToByteArray(string Input)
        {
            return System.Text.Encoding.ASCII.GetBytes(Input);
        }

        public static string ByteArrayToString(byte[] Input)
        {
            return System.Text.Encoding.ASCII.GetString(Input);
        }

        public static string HexStringToString(string Input)
        {
            return ByteArrayToString(HexStringToByteArray(Input));
        }
        #endregion

        #region Integers
        /// <summary>
        /// Encodes an integer to a little endian hex string
        /// </summary>
        public static string IntToHexString(int Input)
        {
            byte[] Bytes = IntToByteArray(Input);
            return ByteArrayToHexString(Bytes);
        }
        public static string UlongToHexString(ulong Input)
        {
            byte[] Bytes = UlongToByteArray(Input);
            return ByteArrayToHexString(Bytes);
        }

        /// <summary>
        /// Decodes an integer from a little endian hex string
        /// </summary>
        public static int HexStringToInt(string Input)
        {
            byte[] Bytes = HexStringToByteArray(Input);
            return ByteArrayToInt(Bytes);
        }
        public static ulong HexStringToUlong(string Input)
        {
            byte[] Bytes = HexStringToByteArray(Input);
            return ByteArrayToUlong(Bytes);
        }

        /// <summary>
        /// Encodes an integer to a little endian byte array
        /// </summary>
        public static byte[] IntToByteArray(int Input)
        {
            byte[] Buffer = new byte[4];
            Buffer[0] = (byte)Input;
            Buffer[1] = (byte)(((uint)Input >> 8) & 0xFF);
            Buffer[2] = (byte)(((uint)Input >> 16) & 0xFF);
            Buffer[3] = (byte)(((uint)Input >> 24) & 0xFF);
            return Buffer;
        }
        public static byte[] UintToByteArray(uint Input)
        {
            byte[] Buffer = new byte[4];
            Buffer[0] = (byte)Input;
            Buffer[1] = (byte)((Input >> 8) & 0xFF);
            Buffer[2] = (byte)((Input >> 16) & 0xFF);
            Buffer[3] = (byte)((Input >> 24) & 0xFF);
            return Buffer;
        }
        public static byte[] UlongToByteArray(ulong Input)
        {
            byte[] Buffer = new byte[8];
            Buffer[0] = (byte)Input;
            Buffer[1] = (byte)((Input >> 8) & 0xFF);
            Buffer[2] = (byte)((Input >> 16) & 0xFF);
            Buffer[3] = (byte)((Input >> 24) & 0xFF);
            Buffer[4] = (byte)((Input >> 32) & 0xFF);
            Buffer[5] = (byte)((Input >> 40) & 0xFF);
            Buffer[6] = (byte)((Input >> 48) & 0xFF);
            Buffer[7] = (byte)((Input >> 56) & 0xFF);
            return Buffer;
        }

        /// <summary>
        /// Decodes an integer from a little endian byte array
        /// </summary>
        public static int ByteArrayToInt(byte[] Input)
        {
            return (Input[3] << 24) | (Input[2] << 16) | (Input[1] << 8) | Input[0];
        }
        public static ulong ByteArrayToUlong(byte[] Input)
        {
            ulong Output = BitConverter.ToUInt64(Input, 0);
            if (!BitConverter.IsLittleEndian)
            {
                Output =    (Output & 0x00000000000000FFUL) << 56 | (Output & 0x000000000000FF00UL) << 40 |
                            (Output & 0x0000000000FF0000UL) << 24 | (Output & 0x00000000FF000000UL) << 8  |
                            (Output & 0x000000FF00000000UL) >> 8  | (Output & 0x0000FF0000000000UL) >> 24 |
                            (Output & 0x00FF000000000000UL) >> 40 | (Output & 0xFF00000000000000UL) >> 56;
            }
            return Output;
        }

        /// <summary>
        /// Encodes an unsigned long to a little endian byte array
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>

        #endregion

        #region Conversion
        /// <summary>
        /// Converts a byte array to a hex string
        /// </summary>
        public static string ByteArrayToHexString(byte[] Input)
        {
            StringBuilder Hex = new StringBuilder(Input.Length * 2);
            foreach (byte Byte in Input) Hex.AppendFormat("{0:x2}", Byte);
            return Hex.ToString();
        }

        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        public static byte[] HexStringToByteArray(String Input)
        {
            byte[] Output = new byte[Input.Length / 2];
            for (int i = 0; i < Input.Length; i += 2)
                Output[i / 2] = Convert.ToByte(Input.Substring(i, 2), 16);
            return Output;
        }
        #endregion

        #region Compression
        /// <summary>
        /// Compresses a byte array
        /// </summary>
        public static byte[] CompressByteArray(byte[] Input)
        {
            using (MemoryStream In = new MemoryStream(Input))
            using (MemoryStream Out = new MemoryStream())
            {
                using (GZipStream Comp = new GZipStream(Out, CompressionMode.Compress))
                    In.CopyTo(Comp);
                return Out.ToArray();
            }
        }

        /// <summary>
        /// Decompresses a byte array
        /// </summary>
        public static byte[] DecompressByteArray(byte[] Input)
        {
            using (MemoryStream In = new MemoryStream(Input))
            using (MemoryStream Out = new MemoryStream())
            {
                using (GZipStream Comp = new GZipStream(In, CompressionMode.Decompress))
                    Comp.CopyTo(Out);
                return Out.ToArray();
            }
        }
        #endregion

        #region Utility
        public static byte[] SplitByteArray(byte[] Source, int Offset, int Length)
        {
            byte[] Output = new byte[Length];
            Buffer.BlockCopy(Source, Offset, Output, 0, Length);
            return Output;
        }

        public static byte[] AppendToByteArray(byte[] Source, byte[] Destination)
        {
            byte[] NewBytes = new byte[Source.Length + Destination.Length];
            Buffer.BlockCopy(Destination, 0, NewBytes, 0, Destination.Length);
            Buffer.BlockCopy(Source, 0, NewBytes, Destination.Length, Source.Length);
            return NewBytes;
        }
        #endregion

        /// <summary>
        /// Gets max block space allowed
        /// </summary>
        public static int BlockSizeLimit()
        {
            // Currently hard set to 115kb
            return 115000;
        }
    }
}
