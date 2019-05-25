//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti
{
    class Daemon
    {
        // Our node instance, interfaced for future extensibility
        static INode Node { get; set; }

        // The ports our node will bind to, set to default values
        static int P2pPort = Globals.P2P_DEFAULT_PORT;
        static int ApiPort = Globals.API_DEFAULT_PORT;

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
                }
            };

            // Parse command line arguments, returns false if any errors occured
            if (!CommandLineParser.Parse(Arguments, Args))
            {
                // Wait for exit
                Logger.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Set a log file, where all console output is saved to
            Logger.LogFile = "cs-turtlecoin.log";

            // Show ascii art, strictly vanity
            Logger.Important(Globals.ASCII_ART);

            // Setting this to true displays a timestamp and label prefix on all log messages
            Logger.ShowPrefix = true;

            // Create a node instance
            Node = new CryptoNote.Node
            {
                // Assign port values (will default to global defaults if not set)
                P2pPort = P2pPort,
                ApiPort = ApiPort
            };

            // Start the node and all associated threads
            if (!Node.Start())
            {
                // Wait for exit
                Logger.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Manually add a peer
            // Node.AddPeer("127.0.0.1", 8099);

            // Wait for exit
            Logger.Important("Press enter at any time to stop node");
            Console.ReadLine();

            // Stops the node instance and all associated threads
            Node.Stop();
        }
    }
}
