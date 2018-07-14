//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Data;
using System;
using System.Collections.Generic;

namespace Canti.Blockchain
{
    class Globals
    {
        // Daemon
        internal static IBlockchainCache[] DAEMON_CHAIN_LEAVES = new IBlockchainCache[] { };
        internal static uint DAEMON_BLOCK_HEIGHT = 1;
        internal static string DAEMON_TOP_ID = Encoding.HexStringToString("0000000000000000000000000000000000000000000000000000000000000000");
        internal static PeerlistEntry[] DAEMON_PEERLIST = new PeerlistEntry[0];
    }
}
