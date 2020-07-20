using System;

namespace FreeSpeak.Packets.Data
{
    public class Handshake2Data : ClientHandshakeData
    {
        public Handshake2Data(uint version, ulong serverLeft, ulong serverRight, uint reversedRandom)
            : base(version, 2)
        {
            ServerLeft = serverLeft;
            ServerRight = serverRight;
            ReversedRandom = reversedRandom;
        }

        public ulong ServerLeft { get; }
        public ulong ServerRight { get; }
        public uint ReversedRandom { get; }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
