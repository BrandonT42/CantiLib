using System;
using System.Collections.Generic;
using System.Text;

// Contains command base structs that can later be inherited
namespace Canti.CryptoNote.P2P
{
    // Contains serialization for command structures
    internal interface ICommandRequestBase
    {
        // Serializes command request to byte array
        byte[] Serialize();
    }
    internal interface ICommandResponseBase<T>
    {
        // Deseriaizes a byte array to a command response
        T Deserialize(byte[] Data);
    }
}
