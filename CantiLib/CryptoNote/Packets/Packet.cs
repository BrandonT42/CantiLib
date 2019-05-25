//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.CryptoNote
{
    internal sealed class Packet
    {
        #region Properties

        public PacketType Type { get; private set; }
        public PacketFlag Flag { get; private set; }
        public PacketHeader Header { get; private set; }
        public PortableStorage Body { get; private set; }

        #endregion

        #region Methods

        public byte[] Serialize()
        {
            byte[] Output = new byte[0];
            byte[] BodyBytes = Body.Serialize();
            Header.PayloadSize = (ulong)BodyBytes.LongLength;
            byte[] HeaderBytes = Header.Serialize();
            Output = Output.AppendBytes(HeaderBytes);
            Output = Output.AppendBytes(BodyBytes);
            return Output;
        }

        #endregion

        #region Constructors

        internal Packet(PacketType Type, PacketFlag Flag, bool ResponseRequired, bool Success = true)
        {
            this.Type = Type;
            this.Flag = Flag;
            Body = new PortableStorage();
            Header = new PacketHeader()
            {
                Signature = 0x0101010101012101UL,
                ProtocolVersion = 1,
                CommandCode = (uint)Type,
                ResponseRequired = ResponseRequired,
                ReturnCode = (uint)(Success ? 0 : 1), // 0 = success
                Flag = (uint)Flag
            };
        }

        internal Packet(PacketHeader Header, PortableStorage Body)
        {
            Type = (PacketType)Header.CommandCode;
            Flag = (PacketFlag)Header.Flag;
            this.Header = Header;
            this.Body = Body;
        }

        /*internal Packet(byte[] Data)
        {
            // Verify packet size
            if (Data.Length < 33)
            {
                throw new ArgumentException("Supplied byte data does not meet minimum header size");
            }

            // Decode packet header
            byte[] HeaderBytes = new byte[33];
            Buffer.BlockCopy(Data, 0, HeaderBytes, 0, 33);
            Header = new PacketHeader(HeaderBytes);
            Type = (PacketType)Header.CommandCode;

            // Get packet body bytes
            byte[] Raw = new byte[Data.Length - 33];
            Buffer.BlockCopy(Data, 33, Raw, 0, Raw.Length);

            // Check payload size
            if (Header.PayloadSize != (ulong)Raw.LongLength)
            {
                // TODO - wait for X amount of time for rest of packet if this doesn't match up
                //throw new ArgumentOutOfRangeException("Packet payload size doesn't match received size");
            }
            else
            {
                // Decode packet body
                Body = new PortableStorage(Raw, out _);
            }
        }*/

        #endregion
    }
}
