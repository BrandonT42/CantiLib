// Contains command base structs that can later be inherited
namespace Canti.Blockchain
{
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
        //T Deserialize(byte[] Data);
    }
}
