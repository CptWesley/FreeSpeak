using System;
using ExtensionNet.Endian;

namespace FreeSpeak.Packets.Data
{
    public class Handshake1Data : PacketData
    {
        public Handshake1Data(ulong serverLeft, ulong serverRight, uint reversedRandom)
        {
            ServerLeft = serverLeft;
            ServerRight = serverRight;
            ReversedRandom = reversedRandom;
        }

        public byte Step => 1;

        public ulong ServerLeft { get; }

        public ulong ServerRight { get; }

        public uint ReversedRandom { get; }

        public override byte[] ToBytes()
        {
            byte[] result = new byte[21];
            result[0] = Step;

            byte[] sl = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ServerLeft.ChangeEndianness()) : BitConverter.GetBytes(ServerLeft);
            Array.Copy(sl, 0, result, 1, 8);

            byte[] sr = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ServerRight.ChangeEndianness()) : BitConverter.GetBytes(ServerRight);
            Array.Copy(sr, 0, result, 9, 8);

            byte[] rnd = BitConverter.IsLittleEndian ? BitConverter.GetBytes(ReversedRandom.ChangeEndianness()) : BitConverter.GetBytes(ReversedRandom);
            Array.Copy(rnd, 0, result, 17, 4);

            return result;
        }
    }
}
