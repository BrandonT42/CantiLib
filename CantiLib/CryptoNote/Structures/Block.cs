using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Canti.Data;

namespace Canti.CryptoNote
{
    [Serializable]
    public class Block
    {
        internal UInt16 MajorVersion = 0;
        internal UInt16 MinorVersion = 0;
        internal UInt64 Timestamp = 0;
        internal string PreviousHash;
        internal UInt32 Nonce = 0;
        internal bool IsAlternative = false;
        internal UInt32 Index = 0;
        internal string Hash;
        internal UInt64 Difficulty = 0;
        internal UInt64 Reward = 0;
        internal UInt64 BaseReward = 0;
        internal UInt64 BlockSize = 0;
        internal UInt64 TransactionsCumulativeSize = 0;
        internal UInt64 AlreadyGeneratedCoins = 0;
        internal UInt64 AlreadyGeneratedTransactions = 0;
        internal UInt64 SizeMedian = 0;
        internal double Penalty = 0.0;
        internal UInt64 TotalFeeAmount = 0;
        internal IList<TransactionDetails> Transactions;

        internal Block(DateTime Timestamp, string PreviousHash, IList<TransactionDetails> Transactions)
        {
            Index = 0;
            this.Timestamp = (UInt64)Timestamp.Ticks;
            this.PreviousHash = PreviousHash;
            this.Transactions = Transactions;
        }

        internal string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes($"{Timestamp}-{PreviousHash ?? ""}-{Serialization.SerializeObjectToJson(Transactions)}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        internal void Mine(Int32 Difficulty)
        {
            var leadingZeros = new string('0', Difficulty);
            while (this.Hash == null || this.Hash.Substring(0, Difficulty) != leadingZeros)
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
            }
        }
    }
}
