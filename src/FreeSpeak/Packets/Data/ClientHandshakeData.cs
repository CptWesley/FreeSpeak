using System;
using System.IO;
using System.Numerics;
using System.Text;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents handshake data sent by clients.
    /// </summary>
    /// <seealso cref="HandshakeData" />
    public abstract class ClientHandshakeData : HandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientHandshakeData"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="step">The step.</param>
        public ClientHandshakeData(uint version, byte step)
            : base(step)
        {
            TeamSpeakVersion = version;
        }

        /// <summary>
        /// Gets the team speak version.
        /// </summary>
        public uint TeamSpeakVersion { get; }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The parsed handshake data.</returns>
        public static ClientHandshakeData Parse(Stream stream)
        {
            uint version = stream.ReadUInt32(Endianness.BigEndian);
            byte step = stream.ReadUInt8();

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

                string command = stream.ReadString(Encoding.UTF8);

                return new Handshake4Data(version, x, n, level, stuff, y, command);
            }

            throw new InvalidOperationException("Invalid step found.");
        }
    }
}
