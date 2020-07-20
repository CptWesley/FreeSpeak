using System.IO;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Third packet of the low level handshake.
    /// </summary>
    /// <seealso cref="ClientHandshakeData" />
    public class Handshake2Data : ClientHandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handshake2Data"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="serverLeft">The server left.</param>
        /// <param name="serverRight">The server right.</param>
        /// <param name="reversedRandom">The reversed random.</param>
        public Handshake2Data(uint version, ulong serverLeft, ulong serverRight, uint reversedRandom)
            : base(version, 2)
        {
            ServerLeft = serverLeft;
            ServerRight = serverRight;
            ReversedRandom = reversedRandom;
        }

        /// <summary>
        /// Gets the left server bytes.
        /// </summary>
        public ulong ServerLeft { get; }

        /// <summary>
        /// Gets the right server bytes.
        /// </summary>
        public ulong ServerRight { get; }

        /// <summary>
        /// Gets the reversed random bits.
        /// </summary>
        public uint ReversedRandom { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(TeamSpeakVersion, Endianness.BigEndian);
            ms.Write(Step);
            ms.Write(ServerLeft, Endianness.BigEndian);
            ms.Write(ServerRight, Endianness.BigEndian);
            ms.Write(ReversedRandom, Endianness.BigEndian);
            return ms.ToArray();
        }
    }
}
