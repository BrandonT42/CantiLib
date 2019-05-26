//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    internal sealed class BlockchainStorage
    {
        #region Properties and Fields

        #region Internal

        // The current height of the blockchain
        internal int Height { get; set; }

        // The hash of the last stored block
        internal string LastHash { get; set; }

        // A byte array representation of the last stored block's hash
        internal byte[] TopId
        {
            get
            {
                return HexStringToByteArray(LastHash);
            }
        }

        #endregion

        #region Private

        // The database we will use to store information
        private IDatabase Database { get; set; }

        #endregion

        #endregion

        #region Methods

        // Starts the blockchain handler and checks database connection
        internal void Start(IDatabase Database)
        {
            // Assign and start database
            this.Database = Database;
            this.Database.Start();

            // TODO - all this
        }

        #endregion

        #region Constructors

        // Initializes a new blockchain storage
        internal BlockchainStorage()
        {
            // Setup default variable values
            Height = 50099;
            LastHash = "0559c2aad34dc8f47b30fefa2652aca9039d6ec0137af3fd0beb16d7c6f91b1d";
        }

        #endregion
    }
}
