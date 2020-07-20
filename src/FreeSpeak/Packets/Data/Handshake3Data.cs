using System;
using System.Numerics;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    public class Handshake3Data : PacketData
    {
        public Handshake3Data(BigInteger x, BigInteger n, uint level, byte[] stuff)
        {
            X = x;
            N = n;
            Level = level;
            ServerStuff = stuff;
        }

        public byte Step => 3;

        public BigInteger X { get; }

        public BigInteger N { get; }

        public uint Level { get; }

        public byte[] ServerStuff { get; }

        public override byte[] ToBytes()
        {
            byte[] result = new byte[233];
            result[0] = Step;
            byte[] x = X.ToByteArray(true, true);
            Array.Copy(x, 0, result, 65 - x.Length, x.Length);
            byte[] n = N.ToByteArray(true, true);
            Array.Copy(n, 0, result, 129 - n.Length, n.Length);

            byte[] level = BitConverter.IsLittleEndian ? BitConverter.GetBytes(Level.ChangeEndianness()) : BitConverter.GetBytes(Level);
            Array.Copy(level, 0, result, 129, 4);

            Array.Copy(ServerStuff, 0, result, 133, 100);

            return result;
        }
    }
}
