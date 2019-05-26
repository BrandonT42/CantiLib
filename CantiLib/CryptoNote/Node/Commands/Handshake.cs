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
        // Handshake request was received
        private void HandleHandshakeRequest(Peer Peer, Packet Packet)
        {
            // Check that peer ID differs from our own
            if (Packet.Body["node_data"]["peer_id"] == Id)
            {
                throw new InvalidOperationException($"Detected connection with self, killing connection...");
            }

            // TODO - check if this peer is already in the list (shouldn't happen? I think it's checked on connect lol)

            // Check version
            if (Packet.Body["node_data"]["version"] < Globals.P2P_MINIMUM_VERSION)
            {
                throw new InvalidOperationException($"Peer at {Peer.Address} attempted to handshake from an outdated version, killing connection...");
            }

            // TODO - check peer time versus local time is within a certain range

            // TODO - validate payload/core sync data

            // Set peer to validated
            Peer.Port = Packet.Body["node_data"]["my_port"];
            Peer.Id = Packet.Body["node_data"]["peer_id"];
            Peer.Validated = true;

            // Let the node know a handshake has been accepted
            OnPeerConnected(Peer);

            // Construct a response packet
            var Response = new Packet(PacketType.HANDSHAKE, PacketFlag.RESPONSE, false);
            Response.Body["node_data"] = new Dictionary<string, dynamic>
            {
                ["network_id"] = Globals.NETWORK_ID,
                ["version"] = Globals.P2P_CURRENT_VERSION,
                ["peer_id"] = Id,
                ["local_time"] = GetTimestamp(),
                ["my_port"] = P2pPort
            };
            Response.Body["payload_data"] = new Dictionary<string, dynamic>
            {
                ["current_height"] = Blockchain.Height,
                ["top_id"] = Blockchain.TopId
            };
            Response.Body["peer_list"] = GetPeerList();

            // Send our response
            Peer.SendMessage(Response);
        }

        // Handshake response was received
        private void HandleHandshakeResponse(Peer Peer, Packet Packet)
        {
            // TODO - validate info
            // TODO - validate peer

            // Set peer to validated
            Peer.Port = Packet.Body["node_data"]["my_port"];
            Peer.Id = Packet.Body["node_data"]["peer_id"];
            Peer.Validated = true;

            // Let the node know a handshake has been accepted
            OnPeerConnected(Peer);
        }

        // A handshake packet was received
        private void HandleHandshake(Peer Peer, Packet Packet)
        {
            // Check if this peer is already validated
            if (Peer.Validated)
            {
                throw new InvalidOperationException($"Received a {Packet.Type} packet from already validated peer {Peer.Address}, killing connection...");
            }

            // Check packet flag
            switch (Packet.Flag)
            {
                case PacketFlag.REQUEST:
                    HandleHandshakeRequest(Peer, Packet);
                    break;

                case PacketFlag.RESPONSE:
                    HandleHandshakeResponse(Peer, Packet);
                    break;
            }
        }

        // Sends a handshake request packet
        private void Handshake(Peer Peer)
        {
            // Check if this peer is already validated
            if (Peer.Validated)
            {
                throw new InvalidOperationException($"Attempted to handshake with already validated peer {Peer.Address}, killing connection...");
            }

            // Construct a request packet
            var Request = new Packet(PacketType.HANDSHAKE, PacketFlag.REQUEST, true);
            Request.Body["node_data"] = new Dictionary<string, dynamic>
            {
                ["network_id"] = Globals.NETWORK_ID,
                ["version"] = Globals.P2P_CURRENT_VERSION,
                ["peer_id"] = Id,
                ["local_time"] = GetTimestamp(),
                ["my_port"] = P2pPort
            };
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
