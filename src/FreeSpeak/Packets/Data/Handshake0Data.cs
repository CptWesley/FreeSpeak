using System;
using System.Collections.Generic;
using System.Text;

namespace FreeSpeak.Packets.Data
{
    public class Handshake0Data : ClientHandshakeData
    {
        public Handshake0Data(uint version, uint timestamp, uint random, ulong reserved)
            : base(version, 0)
        {
            Timestamp = timestamp;
            Random = random;
            Reserved = reserved;
        }

        public uint Timestamp { get; }

        public uint Random { get; }

        public ulong Reserved { get; }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
