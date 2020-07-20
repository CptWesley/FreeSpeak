using System;
using System.Numerics;

namespace FreeSpeak.Packets.Data
{
    public class Handshake4Data : ClientHandshakeData
    {
        public Handshake4Data(uint version, BigInteger x, BigInteger n, uint level, byte[] stuff, BigInteger y)
            : base(version, 4)
        {
            X = x;
            N = n;
            Level = level;
            ServerStuff = stuff;
            Y = y;
        }

        public BigInteger X { get; }
        public BigInteger N { get; }

        public uint Level { get; }

        public byte[] ServerStuff { get; }

        public BigInteger Y { get; }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
