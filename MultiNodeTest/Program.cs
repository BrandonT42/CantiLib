using Canti.CryptoNote;
using System;

namespace Canti
{
    partial class Program
    {
        static INode TrtlNode;
        static INode AthxNode;
        static int TrtlP2pPort = TrtlConfig.P2P_DEFAULT_PORT;
        static int TrtlApiPort = TrtlConfig.API_DEFAULT_PORT;
        static int AthxP2pPort = AthxConfig.P2P_DEFAULT_PORT;
        static int AthxApiPort = AthxConfig.API_DEFAULT_PORT;

        static void Main(string[] Args)
        {
            var Arguments = new CommandLineArguments()
            {
                {
                    "trtl-p2p-port",
                    "Which port to bind P2P server to",
                    typeof(int),
                    (Port) => TrtlP2pPort = Port
                },
                {
                    "trtl-api-port",
                    "Which port to bind API server to",
                    typeof(int),
                    (Port) => TrtlApiPort = Port
                },
                {
                    "trtl-log-level",
                    "Which log level to output at",
                    typeof(int),
                    (Level) =>
                    {
                        if (Level < (int)LogLevel.NONE)
                        {
                            TrtlConfig.LOG_LEVEL = LogLevel.NONE;
                        }
                        else if (Level > (int)LogLevel.MAX)
                        {
                            TrtlConfig.LOG_LEVEL = LogLevel.MAX;
                        }
                        else TrtlConfig.LOG_LEVEL = (LogLevel)Level;
                    }
                },

                {
                    "athx-p2p-port",
                    "Which port to bind P2P server to",
                    typeof(int),
                    (Port) => AthxP2pPort = Port
                },
                {
                    "athx-api-port",
                    "Which port to bind API server to",
                    typeof(int),
                    (Port) => AthxApiPort = Port
                },
                {
                    "athx-log-level",
                    "Which log level to output at",
                    typeof(int),
                    (Level) =>
                    {
                        if (Level < (int)LogLevel.NONE)
                        {
                            AthxConfig.LOG_LEVEL = LogLevel.NONE;
                        }
                        else if (Level > (int)LogLevel.MAX)
                        {
                            AthxConfig.LOG_LEVEL = LogLevel.MAX;
                        }
                        else AthxConfig.LOG_LEVEL = (LogLevel)Level;
                    }
                }
            };

            if (!CommandLineParser.Parse(Arguments, Args))
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            TrtlNode = new Node(TrtlConfig)
            {
                P2pPort = TrtlP2pPort,
                ApiPort = TrtlApiPort
            };
            if (!TrtlNode.Start())
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            AthxNode = new Node(AthxConfig)
            {
                P2pPort = AthxP2pPort,
                ApiPort = AthxApiPort
            };
            if (!AthxNode.Start())
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            //TrtlNode.AddPeer("165.227.252.132", 11897);
            //AthxNode.AddPeer("165.227.252.132", 12000);

            TrtlNode.Logger?.Important("Press enter at any time to stop nodes");
            Console.ReadLine();

            TrtlNode.Stop();
            AthxNode.Stop();
        }
    }
}
