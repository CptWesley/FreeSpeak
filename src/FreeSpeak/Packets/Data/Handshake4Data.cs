using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Fifth packet of the low level handshake.
    /// </summary>
    /// <seealso cref="ClientHandshakeData" />
    public class Handshake4Data : ClientHandshakeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Handshake4Data"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="x">The x.</param>
        /// <param name="n">The n.</param>
        /// <param name="level">The level.</param>
        /// <param name="stuff">The stuff.</param>
        /// <param name="y">The y.</param>
        /// <param name="command">The piggy backed command.</param>
        public Handshake4Data(uint version, BigInteger x, BigInteger n, uint level, byte[] stuff, BigInteger y, string command)
            : base(version, 4)
        {
            X = x;
            N = n;
            Level = level;
            ServerStuff = stuff;
            Y = y;
            Command = command;
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

        /// <summary>
        /// Gets the y.
        /// </summary>
        public BigInteger Y { get; }

        /// <summary>
        /// Gets the piggy-backed command.
        /// </summary>
        public string Command { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(TeamSpeakVersion, Endianness.BigEndian);
            ms.Write(Step);
            ms.Write(X, 64, Endianness.BigEndian);
            ms.Write(N, 64, Endianness.BigEndian);
            ms.Write(Level, Endianness.BigEndian);
            ms.Write(ServerStuff.ToArray());
            ms.Write(Encoding.UTF8.GetBytes(Command));
            return ms.ToArray();
        }
    }
}
