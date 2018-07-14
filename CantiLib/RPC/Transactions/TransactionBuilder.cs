//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using Canti.Blockchain.RPC;
using Canti.Utilities;

namespace Canti.RPC
{
    public class TransactionBuilder
    {
        // Transaction object to build on top of
        TransactionContainer Transaction;

        // Initialization
        public TransactionBuilder()
        {
            Transaction = new TransactionContainer
            {
                Addresses = new List<string>(),
                Anonymity = 0,
                ChangeAddress = "",
                Extra = new Extra(),
                Fee = 0,
                PaymentId = "",
                Transfers = new List<(string, Int64)>(),
                UnlockTime = 0
            };

        }

        // Build transaction and return
        public (bool Success, string Error, string TransactionHash) Send(Connection Wallet)
        {
            // Populate with transaction data
            /*JObject Data = new JObject();
            (string[] Addresses, List<(string Address, Int64 Amount)> Transfers, UInt64 Fee, UInt64 UnlockTime, UInt64 Anonymity, string Extra, string PaymentId, string ChangeAddress) Data;


            if (Transaction.Addresses.Count > 0)
            {
                JArray Addresses = new JArray();
                foreach (string Address in Transaction.Addresses)
                    Addresses.Add(Address);
                Data["addresses"] = Addresses;
            }
            Data["anonymity"] = Transaction.Anonymity;
            if (!string.IsNullOrEmpty(Transaction.ChangeAddress)) Data["changeAddress"] = Transaction.ChangeAddress;
            if (!string.IsNullOrEmpty(Transaction.Extra.ToString())) Data["extra"] = Transaction.Extra.ToString();
            Data["fee"] = Transaction.Fee;
            if (!string.IsNullOrEmpty(Transaction.PaymentId)) Data["paymentId"] = Transaction.PaymentId;
            JArray Transfers = new JArray();
            foreach (KeyValuePair<string, int> Transfer in Transaction.Transfers)
                Transfers.Add(new JObject { ["address"] = Transfer.Key, ["amount"] = Transfer.Value });
            Data["transfers"] = Transfers;
            if (Transaction.UnlockTime > 0) Data["unlockTime"] = Transaction.UnlockTime;

            // Return output string
            return Data.ToString();*/
            return Methods.SendTransaction(Wallet, Transaction.Addresses.ToArray(), Transaction.Transfers, Transaction.Fee, Transaction.UnlockTime, Transaction.Anonymity, Transaction.Extra.ToString(), Transaction.PaymentId, Transaction.ChangeAddress);
        }

        public void AddAddress(string Address)
        {
            Transaction.Addresses.Add(Address);
        }

        public void SetAnonymity(UInt64 Anonymity)
        {
            Transaction.Anonymity = Anonymity;
        }

        public void SetChangeAddress(string Address)
        {
            Transaction.ChangeAddress = Address;
        }

        public void SetExtra(Extra Extra)
        {
            Transaction.Extra = Extra;
        }

        public void SetFee(UInt64 Fee)
        {
            Transaction.Fee = Fee;
        }

        public void SetPaymentId(string PaymentId)
        {
            Transaction.PaymentId = PaymentId;
        }

        public void AddTransfer(string Address, Int64 Amount)
        {
            Transaction.Transfers.Add((Address, Amount));
        }

        public void SetUnlockTime(UInt64 UnlockTime)
        {
            Transaction.UnlockTime = UnlockTime;
        }
    }
}
