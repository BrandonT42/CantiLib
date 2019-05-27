//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    // Handles all blockchain and storage operations
    internal sealed partial class Blockchain
    {
        #region Properties and Fields

        #region Internal

        // Logger used to log messages
        internal Logger Logger { get; set; }

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

        #endregion

        #region Methods

        // Starts the blockchain handler and checks database connection
        internal void Start(IDatabase Database)
        {
            // Start database
            StartDatabase(Database);

            // TODO - all this
        }

        // Stops the blockchain handler and closes the database connection
        internal void Stop()
        {
            // Stop database
            StopDatabase();

            // TODO
        }

        #endregion

        #region Constructors

        // Initializes a new blockchain storage
        internal Blockchain()
        {
            // Setup default variable values
            Height = 50099;
            LastHash = "0559c2aad34dc8f47b30fefa2652aca9039d6ec0137af3fd0beb16d7c6f91b1d";
        }

        #endregion
    }
}
