﻿//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti.Blockchain.Crypto;
using Canti.Utilities;

namespace Canti.Blockchain.Commands
{
    class NotifyRequestChain
    {
        // Command ID
        internal const int Id = GlobalsConfig.CRYPTONOTE_COMMANDS_BASE + 6;

        // Outgoing request structure
        internal struct Request : ICommandRequestBase
        {
            // Variables
            internal string[] BlockIds;

            // Serializes request data into a byte array
            public byte[] Serialize()
            {
                // Create a portable storage
                PortableStorage Storage = new PortableStorage();

                // Add entries
                Storage.AddEntryAsBinary("txs", BlockIds);

                // Return serialized byte array
                return Storage.Serialize();
            }

            // Deseriaizes response data
            public static Request Deserialize(byte[] Data)
            {
                // Deserialize data
                PortableStorage Storage = new PortableStorage();
                Storage.Deserialize(Data);

                // Populate and return new response
                return new Request
                {
                    BlockIds = Hashing.DeserializeHashArray((string)Storage.GetEntry("block_ids"))
                };
            }
        }

        // Process incoming command instance
        internal static void Invoke(LevinProtocol Context, LevinPeer Peer, Command Command)
        {
            // Command is a request
            if (!Command.IsResponse)
            {
                // Deserialize request
                Request Request = Request.Deserialize(Command.Data);

                // debug
                Context.Logger?.Log(Level.DEBUG, "[IN] Received \"Notify Request Chain\" Request:");
                Context.Logger?.Log(Level.DEBUG, "- Response Requested: {0}", !Command.IsNotification);
                Context.Logger?.Log(Level.DEBUG, "- TXs:");
                for (int i = 0; i < Request.BlockIds.Length; i++)
                    Context.Logger?.Log(Level.DEBUG, "  - [{0}]: {1}", i, Request.BlockIds[i]);

                // TODO: Do something with request data
            }
        }
    }
}
