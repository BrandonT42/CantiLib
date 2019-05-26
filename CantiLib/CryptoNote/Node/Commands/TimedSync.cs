//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using static Canti.Utils;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        // Timed sync request was received
        private void HandleTimedSyncRequest(Peer Peer, Packet Packet)
        {
            // TODO - do something with payload data

            // Construct a response packet
            var Response = new Packet(PacketType.TIMED_SYNC, PacketFlag.RESPONSE, false);
            Response.Body["local_time"] = GetTimestamp();
            Response.Body["payload_data"] = new Dictionary<string, dynamic>
            {
                ["current_height"] = Blockchain.Height,
                ["top_id"] = Blockchain.TopId
            };
            Response.Body["local_peerlist"] = GetPeerList();

            // Send our response
            Peer.SendMessage(Response);
        }

        // Timed sync response was received
        private void HandleTimedSyncResponse(Peer Peer, Packet Packet)
        {
            // TODO - check time difference is within range

            // TODO - do something with payload data

            // TODO - do something with peer list data
        }

        // A timed sync packet was received
        private void HandleTimedSync(Peer Peer, Packet Packet)
        {
            // Check if this peer is validated
            if (!Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from non-validated peer {Peer.Address}, killing connection...");
            }

            // Check packet flag
            switch (Packet.Flag)
            {
                case PacketFlag.REQUEST:
                    HandleTimedSyncRequest(Peer, Packet);
                    break;

                case PacketFlag.RESPONSE:
                    HandleTimedSyncResponse(Peer, Packet);
                    break;
            }
        }

        // Sends a timed sync request packet
        private void TimedSync(Peer Peer)
        {
            // Check if this peer is validated
            if (!Peer.Validated) return;

            // Construct a request packet
            var Request = new Packet(PacketType.TIMED_SYNC, PacketFlag.REQUEST, true);
            Request.Body["payload_data"] = new Dictionary<string, dynamic>
            {
                ["current_height"] = Blockchain.Height,
                ["top_id"] = Blockchain.TopId
            };

            // Send our request
            Peer.SendMessage(Request);
        }
    }
}
