//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti
{
    /// <summary>
    /// A SQLite database handler
    /// </summary>
    public sealed class SQLite : IDatabase
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// This database's type
        /// </summary>
        public DatabaseType Type { get; private set; }

        #endregion

        #region Private

        // The file this database will read and write to/from
        private string DatabaseFile { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Starts this database
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Stops this database
        /// </summary>
        public void Stop()
        {

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new SQLite database connection
        /// </summary>
        public SQLite(string DatabaseFile)
        {
            // Set database type
            Type = DatabaseType.SQLITE;

            // Set database file
            this.DatabaseFile = DatabaseFile;
        }

        #endregion
    }
}
