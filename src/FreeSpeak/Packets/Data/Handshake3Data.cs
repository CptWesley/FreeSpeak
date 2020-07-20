using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Fourth packet of the low level handshake.
    /// </summary>
    /// <seealso cref="HandshakeData" />
    public class Handshake3Data : HandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handshake3Data"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="n">The n.</param>
        /// <param name="level">The level.</param>
        /// <param name="stuff">The stuff.</param>
        public Handshake3Data(BigInteger x, BigInteger n, uint level, byte[] stuff)
            : base(3)
        {
            X = x;
            N = n;
            Level = level;
            ServerStuff = stuff;
        }

        /// <summary>
        /// Gets the x.
        /// </summary>
        public BigInteger X { get; }

        /// <summary>
        /// Gets the n.
        /// </summary>
        public BigInteger N { get; }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public uint Level { get; }

        /// <summary>
        /// Gets the server stuff.
        /// </summary>
        public IEnumerable<byte> ServerStuff { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(Step);
            ms.Write(X, 64, Endianness.BigEndian);
            ms.Write(N, 64, Endianness.BigEndian);
            ms.Write(Level, Endianness.BigEndian);
            ms.Write(ServerStuff.ToArray());
            return ms.ToArray();
        }
    }
}
