using System.IO;
using ExtensionNet.Streams;
using ExtensionNet.Types;

namespace FreeSpeak.Protocols.TeamSpeak.Data.Handshakes.Initiation
{
    /// <summary>
    /// Represents the first step in the initiation handshake.
    /// </summary>
    /// <seealso cref="Data" />
    public class Init0 : Data
    {
        /// <summary>
        /// Gets or sets the teamspeak client version as a timestamp.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        public byte Step { get; set; }

        /// <summary>
        /// Gets or sets the current time in unix notation.
        /// </summary>
        public uint Time { get; set; }

        /// <summary>
        /// Gets or sets the randomly generated number.
        /// </summary>
        public uint Random { get; set; }

        /// <summary>
        /// Gets or sets the (zeroes filled) reserved part of the packet.
        /// </summary>
        public ulong Reserved { get; set; }

        /// <summary>
        /// Creates a <see cref="Init0"/> instance from bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A new instance.</returns>
        public static Init0 FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new Init0()
                {
                    Version = stream.ReadUInt32(Endianness.BigEndian),
                    Step = stream.ReadUInt8(),
                    Time = stream.ReadUInt32(Endianness.BigEndian),
                    Random = stream.ReadUInt32(Endianness.BigEndian),
                    Reserved = stream.ReadUInt64(Endianness.BigEndian)
                };
            }
        }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(Version, Endianness.BigEndian);
                stream.Write(Step);
                stream.Write(Time, Endianness.BigEndian);
                stream.Write(Random, Endianness.BigEndian);
                stream.Write(Reserved, Endianness.BigEndian);

                return stream.ToArray();
            }
        }
    }
}
