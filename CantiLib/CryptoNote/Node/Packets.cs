//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        // Handles incoming packets
        internal void HandlePacket(Peer Peer, Packet Packet)
        {
            // Signal that we received a packet
            OnPacketReceived(Peer, Packet);

            // Wrap this in a try-catch, in case we received an invalid packet
            try
            {
                // Handle this packet
                switch (Packet.Type)
                {
                    case PacketType.HANDSHAKE:
                        HandleHandshake(Peer, Packet);
                        break;

                    case PacketType.TIMED_SYNC:
                        HandleTimedSync(Peer, Packet);
                        break;
                }

                // If we reach this far, the packet was valid, set peer
                Peer.LastSeen = GetTimestamp();
            }

            // We use this exception type when a packet contains invalid data
            catch (InvalidOperationException e)
            {
                Logger.Warning(e.Message);
                Peer.Dispose();
            }

            // If this catch fires, the packet was improperly formated
            catch
            {
                Logger.Warning($"Invalid {Packet.Type} packet received from {Peer.Address}, killing connection...");
                Peer.Dispose();
            }
        }
    }
}
