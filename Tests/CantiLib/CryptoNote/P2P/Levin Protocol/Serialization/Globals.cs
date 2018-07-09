using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote.P2P
{
    internal partial class PortableStorage
    {
        // Storage headers
        internal const uint PORTABLE_STORAGE_SIGNATUREA = 0x01011101;
        internal const uint PORTABLE_STORAGE_SIGNATUREB = 0x01020101; // Bender's nightmare 
        internal const byte  PORTABLE_STORAGE_FORMAT_VER = 1;

        // ???
        internal const byte PORTABLE_RAW_SIZE_MARK_MASK = 0x03;
        internal const byte PORTABLE_RAW_SIZE_MARK_BYTE = 0;
        internal const byte PORTABLE_RAW_SIZE_MARK_WORD = 1;
        internal const byte PORTABLE_RAW_SIZE_MARK_DWORD = 2;
        internal const byte PORTABLE_RAW_SIZE_MARK_INT64 = 3;

        // ???
        internal const uint MAX_STRING_LEN_POSSIBLE = 2000000000; // Do not let string be so big

        // ???
        internal const byte SERIALIZE_FLAG_ARRAY = 0x80;

        // Serialization data types
        internal enum SerializationType : byte
        {
            INVALID = 0,  // Invalid type
            LONG    = 1,  // Long
            INT     = 2,  // Int
            SHORT   = 3,  // Short
            SBYTE   = 4,  // Sbyte
            ULONG   = 5,  // Ulong
            UINT    = 6,  // Uint
            USHORT  = 7,  // Ushort
            BYTE    = 8,  // Byte
            DOUBLE  = 9,  // Double
            STRING  = 10, // String
            BOOL    = 11, // Bool
            OBJECT  = 12, // Object
            ARRAY   = 13, // Array
            GUID    = 14  // Guid
        }
    }
}
