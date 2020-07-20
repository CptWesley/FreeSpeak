using System.IO;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// First packet of the low level handshake.
    /// </summary>
    /// <seealso cref="ClientHandshakeData" />
    public class Handshake0Data : ClientHandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handshake0Data"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="random">The random bits.</param>
        /// <param name="reserved">The reserved bits.</param>
        public Handshake0Data(uint version, uint timestamp, uint random, ulong reserved)
            : base(version, 0)
        {
            Timestamp = timestamp;
            Random = random;
            Reserved = reserved;
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public uint Timestamp { get; }

        /// <summary>
        /// Gets the random bits.
        /// </summary>
        public uint Random { get; }

        /// <summary>
        /// Gets the reserved bits.
        /// </summary>
        public ulong Reserved { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(TeamSpeakVersion, Endianness.BigEndian);
            ms.Write(Step);
            ms.Write(Timestamp, Endianness.BigEndian);
            ms.Write(Random, Endianness.BigEndian);
            ms.Write(Reserved, Endianness.BigEndian);
            return ms.ToArray();
        }
    }
}
