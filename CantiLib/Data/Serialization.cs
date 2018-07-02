using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Canti.Data
{
    public class Serialization
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
    }
}
