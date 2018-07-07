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
        internal MethodError error;
        [OptionalField]
        internal UInt32 height;
        [OptionalField]
        internal UInt32 network_height;
        [OptionalField]
        internal string status;
    }

    [Serializable]
    internal class RequestResult
    {
        internal RequestResult() { }
        [OptionalField]
        internal string jsonrpc;
        [OptionalField]
        internal string id;
        [OptionalField]
        internal MethodResult result;
        [OptionalField]
        internal MethodError error;
    }

    [Serializable]
    internal class MethodResult
    {
        internal MethodResult() { }
        [OptionalField]
        internal string viewSecretKey;
        [OptionalField]
        internal string spendSecretKey;
        [OptionalField]
        internal string spendPublicKey;
        [OptionalField]
        internal UInt32 blockCount;
        [OptionalField]
        internal UInt32 knownBlockCount;
        [OptionalField]
        internal string lastBlockHash;
        [OptionalField]
        internal UInt32 peerCount;
        [OptionalField]
        internal string[] addresses;
        [OptionalField]
        internal string address;
        [OptionalField]
        internal UInt64 availableBalance;
        [OptionalField]
        internal UInt64 lockedAmount;
        [OptionalField]
        internal string[] blockHashes;
        [OptionalField]
        internal List<MethodResultItems> items;
        [OptionalField]
        internal string[] transactionHashes;
        [OptionalField]
        internal MethodResultTransaction transaction;
        [OptionalField]
        internal string transactionHash;
        [OptionalField]
        internal UInt64 totalOutputCount;
        [OptionalField]
        internal UInt64 fusionReadyCount;
        [OptionalField]
        internal UInt32 count;
        [OptionalField]
        internal string status;
    }

    [Serializable]
    internal class MethodError
    {
        internal MethodError() { }
        [OptionalField]
        internal string message;
        [OptionalField]
        internal string code;
        [OptionalField]
        internal object data;
    }

    [Serializable]
    internal class MethodResultItems
    {
        internal MethodResultItems() { }
        [OptionalField]
        internal string[] transactionHashes;
        [OptionalField]
        internal string blockHash;
        [OptionalField]
        internal List<MethodResultTransaction> transactions;
    }

    [Serializable]
    internal class MethodResultTransaction
    {
        internal MethodResultTransaction() { }
        [OptionalField]
        internal string transactionHash;
        [OptionalField]
        internal UInt32 blockIndex;
        [OptionalField]
        internal UInt64 timestamp;
        [OptionalField]
        internal bool isBase;
        [OptionalField]
        internal UInt64 unlockTime;
        [OptionalField]
        internal Int64 amount;
        [OptionalField]
        internal UInt64 fee;
        [OptionalField]
        internal string extra;
        [OptionalField]
        internal string paymentId;
        [OptionalField]
        internal List<MethodResultTransfer> transfers;
    }

    [Serializable]
    internal class MethodResultTransfer
    {
        internal MethodResultTransfer() { }
        [OptionalField]
        internal string address;
        [OptionalField]
        internal Int64 amount;
    }
}
