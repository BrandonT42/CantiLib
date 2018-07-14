//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.RPC
{
    internal class TransactionContainer
    {
        internal List<string> Addresses { get; set; }
        internal UInt64 Anonymity { get; set; }
        internal string ChangeAddress { get; set; }
        internal Extra Extra { get; set; }
        internal UInt64 Fee { get; set; }
        internal string PaymentId { get; set; }
        internal List<(string Address, Int64 Amount)> Transfers { get; set; }
        internal UInt64 UnlockTime { get; set; }
    }
}
