//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Canti.Data
{
    public static class Serialization
    {
        public static string SerializeObjectToJson(object Input)
        {
            if (Input == null) throw new ArgumentException("Identifier cannot be null");
            else if (!Input.GetType().IsSerializable) throw new ArgumentException("Identifier must be of a serializable object type");
            using (MemoryStream MemStream = new MemoryStream())
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(Input.GetType());
                Serializer.WriteObject(MemStream, Input);
                byte[] Output = MemStream.ToArray();
                return System.Text.Encoding.UTF8.GetString(Output, 0, Output.Length);
            }
        }

        public static object DeserializeObjectFromJson(string Json)
        {
            object Output = new object();
            using (MemoryStream MemStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Json)))
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(Output.GetType());
                Output = Serializer.ReadObject(MemStream);
                return Output;
            }
        }

        public static T DeserializeObjectFromJson<T>(string Json)
        {
            using (MemoryStream MemStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Json)))
            {
                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)Serializer.ReadObject(MemStream);
            }
        }

        /// <summary>
        /// Gets a sub array from an object array
        /// </summary>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
