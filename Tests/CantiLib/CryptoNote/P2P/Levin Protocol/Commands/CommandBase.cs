using System;
using System.Collections.Generic;
using System.Text;

// Contains command base structs that can later be inherited
namespace Canti.CryptoNote.P2P
{
    // Levin command structure
    internal struct Command
    {
        // Command variables (names should be self explanatory)
        uint CommandCode { get; set; }
        bool IsNotification { get; set; }
        bool IsResponse { get; set; }
        byte[] Data { get; set; }
    }

    // Contains serialization requirement for command request structures
    internal interface ICommandRequestBase
    {
        // Serializes command request to byte array
        byte[] Serialize();
    }

    // Contains deserialization requirement for command response structures
    internal interface ICommandResponseBase<T>
    {
        // Deseriaizes a byte array to a command response
        T Deserialize(byte[] Data);
    }
}
