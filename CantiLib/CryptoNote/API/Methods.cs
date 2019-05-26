//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    // API method context for a node's API server
    internal sealed class ApiMethods : IMethodContext
    {
        #region Properties and Fields

        #region Private

        // A reference to the associated node
        private Node Node { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks that a received request's version is valid
        /// </summary>
        /// <param name="Version">The request's version</param>
        /// <returns>True if version is valid</returns>
        public bool CheckVersion(int Version)
        {
            // Returns true if version is at least the minimum version, but not higher than the current version
            return (Version >= Node.Globals.API_MINIMUM_VERSION && Version <= Node.Globals.API_CURRENT_VERSION);
        }

        #endregion

        #region API Methods

        // TODO - this is a debug method
        [ApiMethod("PeerList")]
        public string GetPeerList()
        {
            return $"{{'peerlist':'{ByteArrayToHexString(Node.GetPeerList())}'}}";
        }

        #endregion

        #region Constructors

        // Initializes this API method context
        internal ApiMethods(Node Node)
        {
            this.Node = Node;
        }

        #endregion
    }
}
