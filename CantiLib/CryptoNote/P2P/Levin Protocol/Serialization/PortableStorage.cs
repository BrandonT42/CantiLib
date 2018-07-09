using Canti.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Canti.CryptoNote.P2P
{
    internal partial class PortableStorage
    {
        // Dictionary of entries
        Dictionary<string, object> Entries = new Dictionary<string, object>();

        // Add an entry
        internal bool AddEntry(string Name, object Value)
        {
            // Get object type
            SerializationType Type = GetType(Value);

            // Add to entry table
            if (Type != SerializationType.INVALID) Entries.Add(Name, Value);

            // Entry is of invalid type
            else return false; // TODO - throw error
            return true;
        }

        // Serializes entry table to a byte array
        internal byte[] Serialize(bool IncludeHeader = true)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Write header
            if (IncludeHeader)
            {
                // Add signatures
                Output = Encoding.AppendToByteArray(Encoding.UintToByteArray(PORTABLE_STORAGE_SIGNATUREA), Output);
                Output = Encoding.AppendToByteArray(Encoding.UintToByteArray(PORTABLE_STORAGE_SIGNATUREB), Output);

                // Add version number
                Output = Encoding.AppendToByteArray(new byte[] { PORTABLE_STORAGE_FORMAT_VER }, Output);
            }

            // Add array length
            Output = Encoding.AppendToByteArray(SerializeVarint((ulong)Entries.Count), Output);

            // Iterate over objects
            foreach (KeyValuePair<string, object> Entry in Entries)
            {
                // Serialize object
                byte[] EntryBytes = SerializeEntry(Entry.Key, Entry.Value);

                // Add to output array
                Output = Encoding.AppendToByteArray(EntryBytes, Output);
            }

            // Return output array
            return Output;
        }

        // Gets an object's serialization type
        internal static SerializationType GetType(object Value)
        {
            // Get serialization type
            SerializationType Type;
            if (Value == null) Type = SerializationType.INVALID;
            else if (Value.GetType() == typeof(long)) Type = SerializationType.LONG;
            else if (Value.GetType() == typeof(int)) Type = SerializationType.INT;
            else if (Value.GetType() == typeof(short)) Type = SerializationType.SHORT;
            else if (Value.GetType() == typeof(sbyte)) Type = SerializationType.SBYTE;
            else if (Value.GetType() == typeof(ulong)) Type = SerializationType.ULONG;
            else if (Value.GetType() == typeof(uint)) Type = SerializationType.UINT;
            else if (Value.GetType() == typeof(ushort)) Type = SerializationType.USHORT;
            else if (Value.GetType() == typeof(byte)) Type = SerializationType.BYTE;
            else if (Value.GetType() == typeof(double)) Type = SerializationType.DOUBLE;
            else if (Value.GetType() == typeof(string)) Type = SerializationType.STRING;
            else if (Value.GetType() == typeof(bool)) Type = SerializationType.BOOL;
            else if (Value.GetType().IsArray) Type = SerializationType.ARRAY;
            else if (Value.GetType() == typeof(Guid)) Type = SerializationType.GUID;
            else Type = SerializationType.OBJECT;
            return Type;
        }

        // Serializes an object to a byte array
        internal static byte[] SerializeObject(object Value, bool IncludeType = true)
        {
            // Create an output array
            byte[] Output = new byte[0];
            
            // Add object type
            SerializationType Type = GetType(Value);
            if (IncludeType) Output = new byte[] { (byte)Type };

            // Create entry bytes buffer
            byte[] EntryBytes = new byte[0];

            // Type is 64 bit
            if (Type == SerializationType.LONG || Type == SerializationType.ULONG)
            {
                // Encode bytes
                ulong Input = Convert.ToUInt64(Value);
                EntryBytes = new byte[8];
                EntryBytes[0] = (byte)Input;
                EntryBytes[1] = (byte)((Input >> 8) & 0xFF);
                EntryBytes[2] = (byte)((Input >> 16) & 0xFF);
                EntryBytes[3] = (byte)((Input >> 24) & 0xFF);
                EntryBytes[4] = (byte)((Input >> 32) & 0xFF);
                EntryBytes[5] = (byte)((Input >> 40) & 0xFF);
                EntryBytes[6] = (byte)((Input >> 48) & 0xFF);
                EntryBytes[7] = (byte)((Input >> 56) & 0xFF);
            }

            // Type is 32 bit
            else if (Type == SerializationType.INT || Type == SerializationType.UINT)
            {
                // Encode bytes
                uint Input = Convert.ToUInt32(Value);
                EntryBytes = new byte[4];
                EntryBytes[0] = (byte)Input;
                EntryBytes[1] = (byte)((Input >> 8) & 0xFF);
                EntryBytes[2] = (byte)((Input >> 16) & 0xFF);
                EntryBytes[3] = (byte)((Input >> 24) & 0xFF);
            }

            // Type is 16 bit
            else if (Type == SerializationType.SHORT || Type == SerializationType.USHORT)
            {
                // Encode bytes
                ushort Input = Convert.ToUInt16(Value);
                EntryBytes = new byte[2];
                EntryBytes[0] = (byte)Input;
                EntryBytes[1] = (byte)((Input >> 8) & 0xFF);
            }

            // Type is 8 bit
            else if (Type == SerializationType.SBYTE || Type == SerializationType.BYTE)
            {
                // Encode bytes
                byte Input = Convert.ToByte(Value);
                EntryBytes = new byte[1];
                EntryBytes[0] = Input;
            }

            // Type is double
            else if (Type == SerializationType.DOUBLE)
            {
                // Encode bytes
                EntryBytes = new byte[8];
                EntryBytes[0] = (byte)(ulong)Value;
                EntryBytes[1] = (byte)(((ulong)Value >> 8) & 0xFF);
                EntryBytes[2] = (byte)(((ulong)Value >> 16) & 0xFF);
                EntryBytes[3] = (byte)(((ulong)Value >> 24) & 0xFF);
                EntryBytes[4] = (byte)(((ulong)Value >> 32) & 0xFF);
                EntryBytes[5] = (byte)(((ulong)Value >> 40) & 0xFF);
                EntryBytes[6] = (byte)(((ulong)Value >> 48) & 0xFF);
                EntryBytes[7] = (byte)(((ulong)Value >> 56) & 0xFF);
            }

            // Type is string
            else if (Type == SerializationType.STRING)
            {
                // Encode bytes
                EntryBytes = Encoding.StringToByteArray((string)Value);

                // Add string length
                EntryBytes = Encoding.AppendToByteArray(EntryBytes, SerializeVarint((ulong)((string)Value).Length));
            }

            // Type is bool
            else if (Type == SerializationType.BOOL)
            {
                // Encode bytes
                EntryBytes = new byte[1] { (bool)Value ? (byte)1 : (byte)0 };
            }

            // Type is object
            else if (Type == SerializationType.OBJECT)
            {
                // Check if object has serialization method
                Type ObjectType = Value.GetType();
                MethodInfo Method = ObjectType.GetMethod("Serialize");
                if (Method == null) throw new Exception("Could not serialize object: No Serialize() method found in object type " + ObjectType.Name);

                // Encode bytes
                EntryBytes = (byte[])Method.Invoke(Value, null); // TODO - Fix this shit ???
            }

            // Type is array
            else if (Type == SerializationType.ARRAY)
            {
                // Encode bytes
                EntryBytes = SerializeArray((Array)Value);
            }

            // Type is guid
            else if (Type == SerializationType.GUID)
            {
                // Encode bytes
                EntryBytes = ((Guid)Value).ToByteArray();

                // Add string length
                EntryBytes = Encoding.AppendToByteArray(EntryBytes, SerializeVarint((ulong)((Guid)Value).ToByteArray().Length));
            }

            // Add bytes to output array
            Output = Encoding.AppendToByteArray(EntryBytes, Output);

            // Return output array
            return Output;
        }

        // Serializes a variable int to a byte array
        internal static byte[] SerializeVarint(ulong Value)
        {
            // Set temporary size
            byte Size = 0x00;

            // Value is 8 bit
            if (Value <= byte.MaxValue)
            {
                // Get byte value
                Size = (byte)(Value << 2);
                Size |= PORTABLE_RAW_SIZE_MARK_BYTE;
            }

            // Value is 16 bit
            else if (Value <= ushort.MaxValue)
            {
                // Get byte value
                Size = (byte)(Value << 2);
                Size |= PORTABLE_RAW_SIZE_MARK_WORD;
            }

            // Value is 32 bit
            else if (Value <= uint.MaxValue)
            {
                // Get byte value
                Size = (byte)(Value << 2);
                Size |= PORTABLE_RAW_SIZE_MARK_DWORD;
            }

            // Value is 64 bit
            else if (Value <= ulong.MaxValue)
            {
                // Get byte value
                Size = (byte)(Value << 2);
                Size |= PORTABLE_RAW_SIZE_MARK_INT64;
            }

            // Create and return a byte array
            return new byte[] { Size };
        }

        // Serializes an array to a byte array
        internal static byte[] SerializeArray(Array Value)
        {
            // Create an output array
            byte[] Output = new byte[0];

            // Add array size
            Output = SerializeVarint((ulong)Value.LongLength);

            // Loop through each array object
            for (int i = 0; i < Value.Length; i++)
            {
                // Read object
                object Child = Value.GetValue(i);

                // Add bytes
                Output = Encoding.AppendToByteArray(SerializeObject(Child), Output);
            }

            // Return output array
            return Output;
        }

        // Serializes an entry to a byte array
        internal static byte[] SerializeEntry(string Name, object Value)
        {
            // Serialize name
            byte[] NameLength = new byte[] { (byte)Name.Length };
            byte[] NameBytes = Encoding.StringToByteArray(Name);
            NameBytes = Encoding.AppendToByteArray(NameBytes, NameLength);

            // Serialize object
            byte[] ObjectBytes = SerializeObject(Value);

            // Return output array
            return Encoding.AppendToByteArray(ObjectBytes, NameBytes);
        }
    }
}
