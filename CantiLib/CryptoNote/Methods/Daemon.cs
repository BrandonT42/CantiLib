using System;
using System.Collections.Generic;
using System.Text;
using Canti.Data;
using Canti.Utilities;

namespace Canti.CryptoNote
{
    public partial class Methods
    {
        /*public static (bool Success, string Error, UInt32 BlockCount, string Status) GetBlockCount(Connection Daemon)
        {
            MethodBuilder Method = new MethodBuilder("getblockcount");
            string Result = SendRequest(Daemon.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, BlockCount: 0, Status: null);
            else return (Success: true, Error: null, BlockCount: Output.result.blockCount, Status: Output.result.status);
        }

        public static (bool Success, string Error, string BlockHash) GetBlockHash(Connection Daemon, UInt32 Height)
        {
            MethodBuilder Method = new MethodBuilder("getblockhash");
            Method.AddExternalParam("height", Height);
            string Result = SendRequest(Daemon.ToString(), Method.ToString());
            RequestResult Output = Serialization.DeserializeObjectFromJson<RequestResult>(Result);
            if (Output.error != null) return (Success: false, Error: Output.error.message, BlockHash: null);
            else return (Success: true, Error: null, BlockHash: Output.ToString());
        }

        public static (bool Success, string Error) GetBlockTemplate(Connection Daemon)
        {

        }

        public static (bool Success, string Error) SubmitBlock(Connection Daemon)
        {

        }

        public static (bool Success, string Error) GetLastBlockHeader(Connection Daemon)
        {

        }

        public static (bool Success, string Error) GetBlockHeaderByHash(Connection Daemon)
        {

        }

        public static (bool Success, string Error) GetBlockHeaderByHeight(Connection Daemon)
        {

        }

        public static (bool Success, string Error) GetCurrencyId(Connection Daemon)
        {

        }

        public static (bool Success, string Error) FGetBlockchainSettings(Connection Daemon)
        {

        }

        public static (bool Success, string Error) FBlocksListJson(Connection Daemon)
        {

        }

        public static (bool Success, string Error) FBlockJson(Connection Daemon)
        {

        }

        public static (bool Success, string Error) FTransactionJson(Connection Daemon)
        {
            
        }*/
    }
}
