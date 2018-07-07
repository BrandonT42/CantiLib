using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote
{
    //typedef boost::variant<BaseInputDetails, KeyInputDetails> TransactionInputDetails;
    [Serializable]
    internal struct TransactionInputDetails { }
    [Serializable]
    internal struct TransactionOutput { }

    [Serializable]
    internal struct TransactionOutputDetails
    {
        internal TransactionOutput output;
        internal UInt64 globalIndex;
    }

    [Serializable]
    internal struct TransactionOutputReferenceDetails
    {
        internal string transactionHash;
        internal Int64 number;
    }

    [Serializable]
    internal struct BaseInputDetails
    {
        internal string input;
        internal UInt64 amount;
    }

    [Serializable]
    internal struct stringDetails
    {
        internal string input;
        internal UInt64 mixin;
        internal TransactionOutputReferenceDetails output;
    }

    [Serializable]
    internal struct TransactionExtraDetails
    {
        internal string publicKey;
        internal byte[] nonce;
        internal byte[] raw;
    }

    [Serializable]
    internal class TransactionDetails
    {
        internal string Hash;
        internal UInt64 Size = 0;
        internal UInt64 Fee = 0;
        internal UInt64 TotalInputsAmount = 0;
        internal UInt64 TotalOutputsAmount = 0;
        internal UInt64 Mixin = 0;
        internal UInt64 UnlockTime = 0;
        internal UInt64 Timestamp = 0;
        internal string PaymentId;
        internal bool HasPaymentId = false;
        internal bool InBlockchain = false;
        internal string BlockHash;
        internal UInt32 BlockIndex = 0;
        internal TransactionExtraDetails Extra;
        internal IList<IList<string>> Signatures;
        internal IList<TransactionInputDetails> Inputs;
        internal IList<TransactionOutputDetails> Outputs;

        // Remove this
        internal string FromAddress;
        internal string ToAddress;
        internal UInt64 Amount;
        internal TransactionDetails(string FromAddress, string ToAddress, UInt64 Amount)
        {
            this.FromAddress = FromAddress;
            this.ToAddress = ToAddress;
            this.Amount = Amount;
        }
    }

    [Serializable]
    internal struct TransactionOutputInformation
    {
        // output info
        internal string type;
        internal UInt64 amount;
        internal UInt32 globalOutputIndex;
        internal UInt32 outputInTransaction;

        // transaction info
        internal string transactionHash;
        internal string transactionPublicKey;

        internal string outputKey;
    }
}
