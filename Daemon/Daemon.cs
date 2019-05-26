//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using System;

namespace CSTurtleCoin
{
    partial class Daemon
    {
        // Our node instance, interfaced for future extensibility
        static INode Node { get; set; }

        // The ports our node will bind to, set to default values
        static int P2pPort = Configuration.P2P_DEFAULT_PORT;
        static int ApiPort = Configuration.API_DEFAULT_PORT;

        static void Main(string[] Args)
        {
            // Create a command line argument list
            var Arguments = new CommandLineArguments()
            {
                // Order is argument name, description, expected type, and an action delegate
                {
                    "p2p-port",
                    "Which port to bind P2P server to",
                    typeof(int),
                    (Port) => P2pPort = Port
                },
                {
                    "api-port",
                    "Which port to bind API server to",
                    typeof(int),
                    (Port) => ApiPort = Port
                },
                {
                    "log-level",
                    "Which log level to output at",
                    typeof(int),
                    (Level) =>
                    {
                        // Level falls below the minimum value, default to minimum
                        if (Level < (int)LogLevel.NONE)
                        {
                            Configuration.LOG_LEVEL = LogLevel.NONE;
                        }

                        // Level falls above the maximum value, default to maximum
                        else if (Level > (int)LogLevel.MAX)
                        {
                            Configuration.LOG_LEVEL = LogLevel.MAX;
                        }

                        // Level falls within range, set to specified level
                        else Configuration.LOG_LEVEL = (LogLevel)Level;
                    }
                }
            };

            // Parse command line arguments, returns false if any errors occured
            if (!CommandLineParser.Parse(Arguments, Args))
            {
                // Wait for exit
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Create a node instance
            Node = new CryptoNote.Node(Configuration)
            {
                // Assign port values
                P2pPort = P2pPort,
                ApiPort = ApiPort
            };

            // Start the node and all associated threads
            if (!Node.Start())
            {
                // Wait for exit
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Manually add a peer
            // Node.AddPeer("127.0.0.1", 8099);

            // Wait for exit
            Node.Logger?.Important("Press enter at any time to stop node");
            Console.ReadLine();

            // Stops the node instance and all associated threads
            Node.Stop();
        }
    }
}
