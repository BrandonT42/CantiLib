using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Canti.Data
{
    public class Encoding
    {
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

        /// <summary>
        /// Encodes an integer to a little endian hex string
        /// </summary>
        public static string IntToHexString(int Input)
        {
            byte[] Bytes = IntToByteArray(Input);
            string Hex = ByteArrayToHexString(Bytes);
            return Hex;
        }

        /// <summary>
        /// Decodes an integer from a little endian hex string
        /// </summary>
        public static int HexStringToInt(string Input)
        {
            byte[] Bytes = HexStringToByteArray(Input);
            int Output = ByteArrayToInt(Bytes);
            return Output;
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

        /// <summary>
        /// Decodes an integer from a little endian byte array
        /// </summary>
        public static int ByteArrayToInt(byte[] Input)
        {
            return (Input[3] << 24) | (Input[2] << 16) | (Input[1] << 8) | Input[0];
        }

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
