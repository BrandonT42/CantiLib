//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Canti;
using Canti.CryptoNote;
using System;

namespace CSTurtleCoin
{
    partial class Daemon
    {
        // Our node instance, interfaced for future extensibility
        private static INode Node { get; set; }

        // The ports our node will bind to, set to default values
        private static int P2pPort = Configuration.P2P_DEFAULT_PORT;
        private static int ApiPort = Configuration.API_DEFAULT_PORT;

        // Main application entry point
        private static void Main(string[] Args)
        {
            // Parse command line arguments, returns false if any errors occured
            if (!CommandLineParser.Parse(Arguments, Args))
            {
                // Wait for exit
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }

            // Create a node instance
            Node = new Node(Configuration)
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
