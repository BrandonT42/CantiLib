using Canti.Data;
using Canti.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Canti.CryptoNote
{
    public partial class Methods
    {
        /*public static (bool Success, string Error, ) GetInfo(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "getinfo");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }*/

        public static (bool Success, string Error, UInt32 Height, UInt32 NetworkHeight, string Status) GetHeight(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "getheight");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, Height: Output.height, Output.network_height, Output.status);
        }

        /*public static (bool Success, string Error, ) GetKnownBlockIds(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "getknownblockids");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) StartMine(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "start_mine");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) StopMine(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "stop_mine");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) GetTransactions(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "gettransactions");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) SendRawTransactions(Connection Daemon)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "sendrawtransactions");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) GetBlocksBin(Connection Daemon, string Destination)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "getblocks.bin");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) GetOIndexesBin(Connection Daemon, string Destination)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "get_o_indexes.bin");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }

        public static (bool Success, string Error, ) GetRandomOutsBin(Connection Daemon, string Destination)
        {
            string Result = ReadEndpoint(Daemon.ToString(true), "getrandom_outs.bin");
            HttpRequestResult Output = Serialization.DeserializeObjectFromJson<HttpRequestResult>(Result);
            return (Success: true, Error: null, );
        }*/
    }
}
