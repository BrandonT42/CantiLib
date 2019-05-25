//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using static Canti.Utils;

namespace Canti.CryptoNote
{
    internal sealed class ApiMethods : IMethodContext
    {
        #region Properties and Fields
        
        private Node Node { get; set; }

        #endregion

        #region Methods

        public bool CheckVersion(int Version)
        {
            return (Version >= Node.Globals.API_MINIMUM_VERSION && Version <= Node.Globals.API_CURRENT_VERSION);
        }

        #endregion

        #region API Methods

        [ApiMethod("PeerList")]
        public string GetPeerList()
        {
            return $"{{'peerlist':'{ByteArrayToHexString(Node.GetPeerList())}'}}";
        }

        #endregion

        #region Constructors

        internal ApiMethods(Node Node)
        {
            this.Node = Node;
        }

        #endregion
    }
}
