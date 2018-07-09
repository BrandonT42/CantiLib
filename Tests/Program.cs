using Canti.Utilities;
using Canti.Data;
using Canti.CryptoNote.P2P;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Canti.Tests
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
            Logger.Log(Level.INFO, "Server started on port {0}, signature of {1} (Encoded and decoded: {2})", Port, LevinProtocol.LEVIN_SIGNATURE, (long)Encoding.ByteArrayToUlong(Encoding.UlongToByteArray(LevinProtocol.LEVIN_SIGNATURE)));

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
                    //Server.Context.NotifyAll(CryptoNote.P2P.Commands.CommandPing.Id, new byte[0]);

                    // Create a response
                    CryptoNote.P2P.Commands.CommandHandshake.Request Request = new CryptoNote.P2P.Commands.CommandHandshake.Request
                    {
                        NodeData = new NodeData()
                        {
                            NetworkId = CryptoNote.P2P.Globals.NETWORK_ID,
                            Version = 1,
                            Port = 8090,
                            LocalTime = (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                            PeerId = BitConverter.ToUInt64(new byte[] { 0x58, 0xfd, 0xde, 0xb8, 0xa4, 0x54, 0x6f, 0xf4 })
                        },
                        PayloadData = new CoreSyncData()
                        {
                            CurrentHeight = 605077,
                            TopId = Encoding.HexStringToString("dd3cc6212ba718412dd17bdba50564f6dc02ed05f81fe9b5a99e0eb0c35d72a0")
                        }
                    };

                    // Get body bytes
                    byte[] BodyBytes = Request.Serialize();

                    // Send notification
                    Server.Context.SendMessageAll(CryptoNote.P2P.Commands.CommandHandshake.Id, BodyBytes);
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
