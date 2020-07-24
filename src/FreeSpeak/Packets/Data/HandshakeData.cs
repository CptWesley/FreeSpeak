using System.IO;
using System.Numerics;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Indicates that the data belongs to a handshake packet.
    /// </summary>
    public abstract class HandshakeData : PacketData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeData"/> class.
        /// </summary>
        /// <param name="step">The step.</param>
        public HandshakeData(byte step)
        {
            Step = step;
        }

        /// <summary>
        /// Gets the step of the handshake.
        /// </summary>
        public byte Step { get; }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The parsed handshake data.</returns>
        public static HandshakeData Parse(Stream stream)
        {
            long pos = stream.Position;
            byte step = stream.ReadUInt8();
            if (step == 1)
            {
                ulong serverLeft = stream.ReadUInt64();
                ulong serverRight = stream.ReadUInt64();
                uint reversedRandom = stream.ReadUInt32();
                return new Handshake1Data(serverLeft, serverRight, reversedRandom);
            }
            else if (step == 3)
            {
                BigInteger x = stream.ReadBigInteger(64);
                BigInteger n = stream.ReadBigInteger(64);
                uint level = stream.ReadUInt32();
                byte[] serverStuff = stream.ReadUInt8(100);
                return new Handshake3Data(x, n, level, serverStuff);
            }

            stream.Position = pos;
            return ClientHandshakeData.Parse(stream);
        }
    }
}
