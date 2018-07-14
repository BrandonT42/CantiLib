//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    internal partial class PortableStorage
    {
        // Constants for varint conversion
        internal const byte PORTABLE_RAW_SIZE_MARK_MASK = 0x03;
        internal const byte PORTABLE_RAW_SIZE_MARK_BYTE = 0;
        internal const byte PORTABLE_RAW_SIZE_MARK_WORD = 1;
        internal const byte PORTABLE_RAW_SIZE_MARK_DWORD = 2;
        internal const byte PORTABLE_RAW_SIZE_MARK_INT64 = 3;

        // Max string length
        internal const uint MAX_STRING_LENGTH = 2000000000;

        // ???
        internal const byte SERIALIZE_FLAG_ARRAY = 0x80;

        // Serialization data types
        internal enum SerializationType : byte
        {
            INVALID = 0,  // Invalid type
            LONG = 1,  // Long
            INT = 2,  // Int
            SHORT = 3,  // Short
            SBYTE = 4,  // Sbyte
            ULONG = 5,  // Ulong
            UINT = 6,  // Uint
            USHORT = 7,  // Ushort
            BYTE = 8,  // Byte
            DOUBLE = 9,  // Double
            STRING = 10, // String
            BOOL = 11, // Bool
            OBJECT = 12, // Object
            ARRAY = 13 // Array
        }
    }
}
