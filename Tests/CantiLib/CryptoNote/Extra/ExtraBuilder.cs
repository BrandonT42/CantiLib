using System;
using System.Collections.Generic;
using Canti.Data;

namespace Canti.CryptoNote
{
    public class ExtraBuilder
    {
        // Extra object to build on top of
        private Extra Extra;

        // Initialization
        public ExtraBuilder()
        {
            Extra = new Extra
            {
                Children = new List<object>()
            };
        }

        // Build extra and return
        public Extra Build(bool Compress = false)
        {
            // Encode object list to hex data
            foreach (object Child in Extra.Children)
            {
                byte[] Children = Encoding.ObjectToByteArray(Child);
                if (Compress) Children = Encoding.CompressByteArray(Children);
                Extra.Hex += "02" + Encoding.IntToHexString(Children.Length) + Encoding.ByteArrayToHexString(Children);
            }

            // Check extra length against block size
            if (Extra.Hex.Length / 2 > Encoding.BlockSizeLimit())
                throw new Exception("Extra size exceeds block limit");

            // Output extra
            return Extra;
        }

        // Add a child
        public void Add(object Object)
        {
            if (Object == null) throw new ArgumentException("Value cannot be null");
            else if (!Object.GetType().IsSerializable) throw new ArgumentException("Value must be of a serializable object type");
            else Extra.Children.Add(Object);
        }
    }
}
