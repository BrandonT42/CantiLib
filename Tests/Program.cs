using Canti.Utilities;
using Canti.Data;
using System;
using System.Net.Sockets;
using Canti.CryptoNote.P2P;
using System.Collections.Generic;

namespace BlockchainTest
{
    class Program
    {
        public static int Port = 8091;
        public static Server Server = new Server();
        public static Logger Logger = new Logger();

        static void DataReceived(object sender, EventArgs e)
        {
            // Custom incoming data handling
            Packet Packet = (Packet)sender;
            Logger.Log(Level.DEBUG, "Received packet from {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        static void DataSent(object sender, EventArgs e)
        {
            // Custom outgoing data handling
            Packet Packet = (Packet)sender;
            Logger.Log(Level.DEBUG, "Sent packet to {0}: {1}", Packet.Peer.Address, Encoding.ByteArrayToHexString(Packet.Data));
        }

        static void ServerError(object sender, EventArgs e)
        {
            // An error was received
            Logger.Log(Level.ERROR, "Server error: {0}", (string)sender);
        }

        static void PeerConnected(object sender, EventArgs e)
        {
            // Custom peer connected handling
            PeerConnection Peer = (PeerConnection)sender;
            Logger.Log(Level.DEBUG, "Peer connection formed with {0}", Peer.Address);
        }

        static void PeerDisconnected(object sender, EventArgs e)
        {
            // Custom peer disconnected handling
            PeerConnection Peer = (PeerConnection)sender;
            Logger.Log(Level.DEBUG, "Peer connection lost with {0}", Peer.Address);
        }

        static void Main(string[] args)
        {
            // Parse commandline arguments
            if (args.Length >= 1) Port = int.Parse(args[0]);

            // Set log level and start logger
            Logger.LogLevel = Level.DEBUG;
            Logger.Start();
            
            // CBind event handlers
            Server.OnDataReceived += DataReceived;
            Server.OnDataSent += DataSent;
            Server.OnError += ServerError;
            Server.OnPeerConnected += PeerConnected;

            // Start server
            Server.Start(Port);
            Logger.Log(Level.INFO, "Server started on port {0}", Port);

            // Enter into a loop
            int MenuSelection = 0;
            while (MenuSelection != 4)
            {
                // Manually connect to a peer
                if (MenuSelection == 1)
                {
                    Logger.Log(Level.INFO, "Enter a URL:");
                    string Url = Console.ReadLine();
                    Logger.Log(Level.INFO, "Enter a port:");
                    int Port = int.Parse(Console.ReadLine());
                    Server.Connect(new Connection(Url, Port, ""));

                }

                // Broadcast a test packet
                else if (MenuSelection == 2)
                {
                    Server.Context.NotifyAll(Canti.CryptoNote.P2P.Commands.CommandPing.Id, new byte[0]);
                }

                // Show peer list
                else if (MenuSelection == 3)
                {
                    Server.Prune();
                    string Peers = "";
                    List<PeerConnection> PeerList = Server.GetPeerList();
                    foreach (PeerConnection Peer in PeerList) Peers += Peer.Address + " ";
                    Logger.Log(Level.DEBUG, "Peers:");
                    Logger.Log(Level.DEBUG, Peers);
                }

                // Write menu
                Logger.Log(Level.INFO, "Menu:");
                Logger.Log(Level.INFO, " 1\tConnect to a Server");
                Logger.Log(Level.INFO, " 2\tTest Packet");
                Logger.Log(Level.INFO, " 3\tShow Peer List");
                Logger.Log(Level.INFO, " 4\tExit");
                Logger.Log(Level.INFO, "Enter Selection:");

                // Get menu selection
                MenuSelection = int.Parse(Console.ReadLine());
            }

            // Close all connections
            Server.Close();
        }
    }
}
