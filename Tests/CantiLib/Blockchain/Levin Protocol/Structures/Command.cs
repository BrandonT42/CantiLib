//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Blockchain
{
    // Levin command structure
    internal struct Command
    {
        // Command variables (names should be self explanatory)
        internal uint CommandCode { get; set; }
        internal bool IsNotification { get; set; }
        internal bool IsResponse { get; set; }
        internal byte[] Data { get; set; }
    }
}
