using System;
using System.IO;
using System.Numerics;
using ExtensionNet.Streams;
using ExtensionNet.Types;

namespace FreeSpeak.Packets.Data
{
    public abstract class ClientHandshakeData : PacketData
    {
        public ClientHandshakeData(uint version, byte step)
        {
            TeamSpeakVersion = version;
            Step = step;
        }

        public uint TeamSpeakVersion { get; }

        public byte Step { get; }

        public static ClientHandshakeData Parse(Stream stream)
        {
            uint version = stream.ReadUInt32(Endianness.BigEndian);
            byte step = stream.ReadUInt8();

            Console.WriteLine($"Step: {step}");

            if (step == 0)
            {
                uint timestamp = stream.ReadUInt32(Endianness.BigEndian);
                uint random = stream.ReadUInt32(Endianness.BigEndian);
                ulong reserved = stream.ReadUInt64(Endianness.BigEndian);
                return new Handshake0Data(version, timestamp, random, reserved);
            }
            else if (step == 2)
            {
                ulong sl = stream.ReadUInt64(Endianness.BigEndian);
                ulong sr = stream.ReadUInt64(Endianness.BigEndian);
                uint reversedRandom = stream.ReadUInt32(Endianness.BigEndian);
                Console.Write($"Found rnd: {reversedRandom}");
                return new Handshake2Data(version, sl, sr, reversedRandom);
            }
            else if (step == 4)
            {
                byte[] xb = stream.ReadUInt8(64);
                byte[] nb = stream.ReadUInt8(64);
                uint level = stream.ReadUInt32(Endianness.BigEndian);
                byte[] stuff = stream.ReadUInt8(100);
                byte[] yb = stream.ReadUInt8(64);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(xb);
                    Array.Reverse(nb);
                    Array.Reverse(yb);
                }

                BigInteger x = new BigInteger(xb);
                BigInteger n = new BigInteger(nb);
                BigInteger y = new BigInteger(yb);

                BigInteger yx = BigInteger.ModPow(x, BigInteger.Pow(2, (int)level), n);

                Console.WriteLine($"Actual y: {y} should b: {yx}");

                return new Handshake4Data(version, x, n, level, stuff, y);
            }

            throw new InvalidOperationException("Invalid step found.");
        }
    }
}
