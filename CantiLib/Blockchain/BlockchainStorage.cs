//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti
{
    internal sealed class BlockchainStorage
    {
        public int Height { get; set; }
        public byte[] TopId { get; set; }

        public BlockchainStorage()
        {
            Height = 50099;
            TopId = HexStringToByteArray("0559c2aad34dc8f47b30fefa2652aca9039d6ec0137af3fd0beb16d7c6f91b1d");
        }
    }
}
