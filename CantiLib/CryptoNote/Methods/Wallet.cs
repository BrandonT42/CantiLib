using System;
using System.Collections.Generic;
using System.Text;
using Canti.Data;
using Canti.Utilities;

namespace Canti.CryptoNote
{
    public partial class Methods
    {
        public static (bool Success, string Error) Reset(Connection Wallet, string ViewSecretKey = "")
        {
            MethodBuilder Method = new MethodBuilder("reset", Wallet.Password);
            if (!String.IsNullOrEmpty(ViewSecretKey)) Method.AddParam("viewSecretKey", ViewSecretKey);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null)
            {
                return (Success: false, Error: Output.error.message);
            }
            else return (Success: true, Error: null);
        }

        public static (bool Success, string Error) Save(Connection Wallet)
        {
            MethodBuilder Method = new MethodBuilder("save", Wallet.Password);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message);
            else return (Success: true, Error: null);
        }

        public static (bool Success, string Error, string ViewKey) GetViewKey(Connection Wallet)
        {
            MethodBuilder Method = new MethodBuilder("getViewKey", Wallet.Password);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, ViewKey: null);
            else return (Success: true, Error: null, ViewKey: Output.result.viewSecretKey);
        }

        public static (bool Success, string Error, string SecretKey, string PublicKey) GetSpendKeys(Connection Wallet, string Address)
        {
            MethodBuilder Method = new MethodBuilder("getSpendKeys", Wallet.Password);
            Method.AddParam("address", Address);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, SecretKey: null, PublicKey: null);
            else return (Success: true, Error: null, SecretKey: Output.result.spendSecretKey, PublicKey: Output.result.spendPublicKey);
        }

        public static (bool Success, string Error, UInt32 BlockCount, UInt32 KnownBlockCount, string LastBlockHash, UInt32 PeerCount) GetStatus(Connection Wallet)
        {
            MethodBuilder Method = new MethodBuilder("getStatus", Wallet.Password);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, BlockCount: 0, KnownBlockCount: 0, LastBlockHash: null, PeerCount: 0);
            else return (Success: true, Error: null, BlockCount: Output.result.blockCount, KnownBlockCount: Output.result.knownBlockCount, Output.result.lastBlockHash, Output.result.peerCount);
        }

        public static (bool Success, string Error, string[] Addresses) GetAddresses(Connection Wallet)
        {
            MethodBuilder Method = new MethodBuilder("getAddresses", Wallet.Password);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, Addresses: null);
            else return (Success: true, Error: null, Addresses: Output.result.addresses);
        }

        public static (bool Success, string Error, string Address) CreateAddress(Connection Wallet, string SecretSpendKey = "", string PublicSpendKey = "")
        {
            MethodBuilder Method = new MethodBuilder("createAddress", Wallet.Password);
            if (!String.IsNullOrEmpty(SecretSpendKey)) Method.AddParam("secretSpendKey", SecretSpendKey);
            if (!String.IsNullOrEmpty(PublicSpendKey)) Method.AddParam("publicSpendKey", PublicSpendKey);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, Address: null);
            else return (Success: true, Error: null, Address: Output.result.address);
        }

        public static (bool Success, string Error) DeleteAddress(Connection Wallet, string Address)
        {
            MethodBuilder Method = new MethodBuilder("deleteAddress", Wallet.Password);
            Method.AddParam("address", Address);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message);
            else return (Success: true, Error: null);
        }

        public static (bool Success, string Error, UInt64 AvailableBalance, UInt64 LockedAmount) GetBalance(Connection Wallet, string Address = "")
        {
            MethodBuilder Method = new MethodBuilder("getBalance", Wallet.Password);
            if (!String.IsNullOrEmpty(Address)) Method.AddParam("address", Address);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, AvailableBalance: 0, LockedAmount: 0);
            else return (Success: true, Error: null, AvailableBalance: Output.result.availableBalance, LockedAmount: Output.result.lockedAmount);
        }

        public static (bool Success, string Error, string[] BlockHashes) GetBlockHashes(Connection Wallet, UInt32 FirstBlockIndex, UInt32 BlockCount)
        {
            MethodBuilder Method = new MethodBuilder("getBlockHashes", Wallet.Password);
            Method.AddParam("firstBlockIndex", FirstBlockIndex);
            Method.AddParam("blockCount", BlockCount);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, BlockHashes: null);
            else return (Success: true, Error: null, BlockHashes: Output.result.blockHashes);
        }

        public static (bool Success, string Error, List<(string[] TransactionHashes, string BlockHash)> Items) GetTransactionHashes(Connection Wallet, string Address = "", string BlockHash = "", UInt32 FirstBlockIndex = 0, UInt32 BlockCount = 0, string PaymentId = "")
        {
            MethodBuilder Method = new MethodBuilder("getTransactionHashes", Wallet.Password);
            if (!String.IsNullOrEmpty(Address)) Method.AddParam("address", Address);
            if (FirstBlockIndex > 0) Method.AddParam("firstBlockIndex", FirstBlockIndex);
            if (!String.IsNullOrEmpty(BlockHash)) Method.AddParam("blockHash", BlockHash);
            Method.AddParam("blockCount", BlockCount);
            if (!String.IsNullOrEmpty(PaymentId)) Method.AddParam("paymentId", PaymentId);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, Items: null);
            else
            {
                List<(string[] TransactionHashes, string BlockHash)> Items = new List<(string[] TransactionHashes, string BlockHash)>();
                foreach (var Item in Output.result.items)
                    Items.Add((Item.transactionHashes, Item.blockHash));
                return (Success: true, Error: null, Items: Items);
            }
        }

        public static (bool Success, string Error, List<(string BlockHash, List<(string TransactionHash, UInt32 BlockIndex, UInt64 Timestamp, bool IsBase, UInt64 UnlockTime, Int64 Amount, UInt64 Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers)> Transactions)> Items) GetTransactions(Connection Wallet, string[] Addresses = null, string BlockHash = null, UInt32 FirstBlockIndex = 0, UInt32 BlockCount = 0, string PaymentId = null)
        {
            MethodBuilder Method = new MethodBuilder("getTransactions", Wallet.Password);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            if (!String.IsNullOrEmpty(BlockHash)) Method.AddParam("blockHash", BlockHash);
            if (FirstBlockIndex > 0) Method.AddParam("firstBlockIndex", FirstBlockIndex);
            if (BlockCount > 0) Method.AddParam("blockCount", BlockCount);
            if (!String.IsNullOrEmpty(PaymentId)) Method.AddParam("paymentId", PaymentId);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, Items: null);
            else
            {
                List<(string BlockHash, List<(string TransactionHash, UInt32 BlockIndex, UInt64 Timestamp, bool IsBase, UInt64 UnlockTime, Int64 Amount, UInt64 Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers)> Transactions)> Items = new List<(string BlockHash, List<(string TransactionHash, uint BlockIndex, ulong Timestamp, bool IsBase, ulong UnlockTime, Int64 Amount, ulong Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers)> Transactions)>();
                foreach (var Item in Output.result.items)
                {
                    List<(string TransactionHash, UInt32 BlockIndex, UInt64 Timestamp, bool IsBase, UInt64 UnlockTime, Int64 Amount, UInt64 Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers)> Transactions = new List<(string TransactionHash, uint BlockIndex, ulong Timestamp, bool IsBase, ulong UnlockTime, Int64 Amount, ulong Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers)>();
                    foreach (var Transaction in Item.transactions)
                    {
                        List<(string Address, Int64 Amount)> Transfers = new List<(string Address, Int64 Amount)>();
                        foreach (var Transfer in Transaction.transfers)
                            Transfers.Add((Transfer.address, Transfer.amount));
                        Transactions.Add((Transaction.transactionHash, Transaction.blockIndex, Transaction.timestamp, Transaction.isBase, Transaction.unlockTime, Transaction.amount, Transaction.fee, Transaction.extra, Transaction.paymentId, Transfers));
                    }
                    Items.Add((Item.blockHash, Transactions));
                }
                return (Success: true, Error: null, Items: Items);
            }
        }

        public static (bool Success, string Error, string[] TransactionHashes) GetUnconfirmedTransactionHashes(Connection Wallet, string[] Addresses = null)
        {
            MethodBuilder Method = new MethodBuilder("getUnconfirmedTransactionHashes", Wallet.Password);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHashes: null);
            else return (Success: true, Error: null, TransactionHashes: Output.result.transactionHashes);
        }

        public static (bool Success, string Error, string TransactionHash, UInt32 BlockIndex, UInt64 Timestamp, bool IsBase, UInt64 UnlockTime, Int64 Amount, UInt64 Fee, string Extra, string PaymentId, List<(string Address, Int64 Amount)> Transfers) GetTransaction(Connection Wallet, string TransactionHash)
        {
            MethodBuilder Method = new MethodBuilder("getTransaction", Wallet.Password);
            Method.AddParam("transactionHash", TransactionHash);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHash: null, BlockIndex: 0, Timestamp: 0, IsBase: false, UnlockTime: 0, Amount: 0, Fee: 0, Extra: null, PaymentId: null, Transfers: null);
            else
            {
                List<(string Address, Int64 Amount)> Transfers = new List<(string Address, Int64 Amount)>();
                foreach (var Transfer in Output.result.transaction.transfers)
                    Transfers.Add((Transfer.address, Transfer.amount));
                return (Success: true, Error: null, TransactionHash: Output.result.transaction.transactionHash, BlockIndex: Output.result.transaction.blockIndex, Timestamp: Output.result.transaction.timestamp, IsBase: Output.result.transaction.isBase, UnlockTime: Output.result.transaction.unlockTime, Amount: Output.result.transaction.amount, Fee: Output.result.transaction.fee, Extra: Output.result.transaction.extra, PaymentId: Output.result.transaction.paymentId, Transfers);
            }
        }

        public static (bool Success, string Error, string TransactionHash) SendTransaction(Connection Wallet, string[] Addresses = null, List<(string Address, Int64 Amount)> Transfers = null, UInt64 Fee = 0, UInt64 UnlockTime = 0, UInt64 Anonymity = 0, string Extra = null, string PaymentId = null, string ChangeAddress = null)
        {
            MethodBuilder Method = new MethodBuilder("sendTransaction", Wallet.Password);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            if (Transfers != null) Method.AddParam("transfers", Transfers);
            Method.AddParam("fee", Fee);
            if (UnlockTime > 0) Method.AddParam("unlockTime", UnlockTime);
            Method.AddParam("anonymity", Anonymity);
            if (!String.IsNullOrEmpty(Extra)) Method.AddParam("extra", Extra);
            if (!String.IsNullOrEmpty(PaymentId)) Method.AddParam("paymentId", PaymentId);
            if (!String.IsNullOrEmpty(ChangeAddress)) Method.AddParam("changeAddress", ChangeAddress);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHash: null);
            else return (Success: true, Error: null, TransactionHash: Output.result.transactionHash);
        }

        public static (bool Success, string Error, string TransactionHash) CreateDelayedTransaction(Connection Wallet, string[] Addresses = null, List<(string Address, Int64 Amount)> Transfers = null, UInt64 Fee = 0, UInt64 UnlockTime = 0, UInt64 Anonymity = 0, string Extra = null, string PaymentId = null, string ChangeAddress = null)
        {
            MethodBuilder Method = new MethodBuilder("createDelayedTransaction", Wallet.Password);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            if (Transfers != null) Method.AddParam("transfers", Transfers);
            Method.AddParam("fee", Fee);
            if (UnlockTime > 0) Method.AddParam("unlockTime", UnlockTime);
            Method.AddParam("anonymity", Anonymity);
            if (!String.IsNullOrEmpty(Extra)) Method.AddParam("extra", Extra);
            if (!String.IsNullOrEmpty(PaymentId)) Method.AddParam("paymentId", PaymentId);
            if (!String.IsNullOrEmpty(ChangeAddress)) Method.AddParam("changeAddress", ChangeAddress);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHash: null);
            else return (Success: true, Error: null, TransactionHash: Output.result.transactionHash);
        }

        public static (bool Success, string Error, string[] TransactionHashes) GetDelayedTransactionHashes(Connection Wallet)
        {
            MethodBuilder Method = new MethodBuilder("getDelayedTransactionHashes", Wallet.Password);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHashes: null);
            else return (Success: true, Error: null, TransactionHashes: Output.result.transactionHashes);
        }

        public static (bool Success, string Error) DeleteDelayedTransaction(Connection Wallet, string TransactionHash)
        {
            MethodBuilder Method = new MethodBuilder("deleteDelayedTransaction", Wallet.Password);
            Method.AddParam("transactionHash", TransactionHash);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message);
            else return (Success: true, Error: null);
        }

        public static (bool Success, string Error) SendDelayedTransaction(Connection Wallet, string TransactionHash)
        {
            MethodBuilder Method = new MethodBuilder("sendDelayedTransaction", Wallet.Password);
            Method.AddParam("transactionHash", TransactionHash);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message);
            else return (Success: true, Error: null);
        }

        public static (bool Success, string Error, string TransactionHash) SendFusionTransaction(Connection Wallet, UInt64 Threshold, UInt64 Anonymity, string[] Addresses, string DestinationAddress)
        {
            MethodBuilder Method = new MethodBuilder("sendFusionTransaction", Wallet.Password);
            Method.AddParam("threshold", Threshold);
            Method.AddParam("anonymity", Anonymity);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            if (!String.IsNullOrEmpty(DestinationAddress)) Method.AddParam("destinationAddress", DestinationAddress);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TransactionHash: null);
            else return (Success: true, Error: null, TransactionHash: Output.result.transactionHash);
        }

        public static (bool Success, string Error, UInt64 TotalOutputCount, UInt64 FusionReadyCount) EstimateFusion(Connection Wallet, UInt64 Threshold, string[] Addresses = null)
        {
            MethodBuilder Method = new MethodBuilder("estimateFusion", Wallet.Password);
            Method.AddParam("threshold", Threshold);
            if (Addresses != null) Method.AddParam("addresses", Addresses);
            string Result = SendRequest(Wallet.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, TotalOutputCount: 0, FusionReadyCount: 0);
            else return (Success: true, Error: null, TotalOutputCount: Output.result.totalOutputCount, FusionReadyCount: Output.result.fusionReadyCount);
        }
    }
}
