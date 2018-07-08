using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Canti.CryptoNote.RPC
{
    [Serializable]
    internal class HttpRequestResult
    {
        internal HttpRequestResult() { }
        [OptionalField]
        internal MethodError error = null;
        [OptionalField]
        internal UInt32 height = 0;
        [OptionalField]
        internal UInt32 network_height = 0;
        [OptionalField]
        internal string status = null;
    }

    [Serializable]
    internal class RequestResult
    {
        internal RequestResult() { }
        [OptionalField]
        internal string jsonrpc = null;
        [OptionalField]
        internal string id = null;
        [OptionalField]
        internal MethodResult result = null;
        [OptionalField]
        internal MethodError error = null;
    }

    [Serializable]
    internal class MethodResult
    {
        internal MethodResult() { }
        [OptionalField]
        internal string viewSecretKey = null;
        [OptionalField]
        internal string spendSecretKey = null;
        [OptionalField]
        internal string spendPublicKey = null;
        [OptionalField]
        internal UInt32 blockCount = 0;
        [OptionalField]
        internal UInt32 knownBlockCount = 0;
        [OptionalField]
        internal string lastBlockHash = null;
        [OptionalField]
        internal UInt32 peerCount = 0;
        [OptionalField]
        internal string[] addresses = null;
        [OptionalField]
        internal string address = null;
        [OptionalField]
        internal UInt64 availableBalance = 0;
        [OptionalField]
        internal UInt64 lockedAmount = 0;
        [OptionalField]
        internal string[] blockHashes = null;
        [OptionalField]
        internal List<MethodResultItems> items = null;
        [OptionalField]
        internal string[] transactionHashes = null;
        [OptionalField]
        internal MethodResultTransaction transaction = null;
        [OptionalField]
        internal string transactionHash = null;
        [OptionalField]
        internal UInt64 totalOutputCount = 0;
        [OptionalField]
        internal UInt64 fusionReadyCount = 0;
        [OptionalField]
        internal UInt32 count = 0;
        [OptionalField]
        internal string status = null;
    }

    [Serializable]
    internal class MethodError
    {
        internal MethodError() { }
        [OptionalField]
        internal string message = null;
        [OptionalField]
        internal string code = null;
        [OptionalField]
        internal object data = null;
    }

    [Serializable]
    internal class MethodResultItems
    {
        internal MethodResultItems() { }
        [OptionalField]
        internal string[] transactionHashes = null;
        [OptionalField]
        internal string blockHash = null;
        [OptionalField]
        internal List<MethodResultTransaction> transactions = null;
    }

    [Serializable]
    internal class MethodResultTransaction
    {
        internal MethodResultTransaction() { }
        [OptionalField]
        internal string transactionHash = null;
        [OptionalField]
        internal UInt32 blockIndex = 0;
        [OptionalField]
        internal UInt64 timestamp = 0;
        [OptionalField]
        internal bool isBase = false;
        [OptionalField]
        internal UInt64 unlockTime = 0;
        [OptionalField]
        internal Int64 amount = 0;
        [OptionalField]
        internal UInt64 fee = 0;
        [OptionalField]
        internal string extra = null;
        [OptionalField]
        internal string paymentId = null;
        [OptionalField]
        internal List<MethodResultTransfer> transfers = null;
    }

    [Serializable]
    internal class MethodResultTransfer
    {
        internal MethodResultTransfer() { }
        [OptionalField]
        internal string address = null;
        [OptionalField]
        internal Int64 amount = 0;
    }
}
