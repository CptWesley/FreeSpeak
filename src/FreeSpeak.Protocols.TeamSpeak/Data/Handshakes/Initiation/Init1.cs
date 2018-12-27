using System.Diagnostics.CodeAnalysis;
using System.IO;
using ExtensionNet.Streams;
using ExtensionNet.Types;

namespace FreeSpeak.Protocols.TeamSpeak.Data.Handshakes.Initiation
{
    /// <summary>
    /// Represents the second step in the initiation handshake.
    /// </summary>
    /// <seealso cref="Data" />
    public class Init1 : Data
    {
        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        public byte Step { get; set; }

        /// <summary>
        /// Gets or sets the server data.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "We don't mind changing the entire array.")]
        public byte[] ServerData { get; set; }

        /// <summary>
        /// Gets or sets the randomly generated number.
        /// </summary>
        public uint Random { get; set; }

        /// <summary>
        /// Creates a <see cref="Init1"/> instance from bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A new instance.</returns>
        public static Init1 FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new Init1()
                {
                    Step = stream.ReadUInt8(),
                    ServerData = stream.ReadUInt8(16),
                    Random = stream.ReadUInt32(Endianness.BigEndian)
                };
            }
        }

        /// <inheritdoc/>
        public override byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(Step);
                stream.Write(ServerData);
                stream.Write(Random, Endianness.BigEndian);

                return stream.ToArray();
            }
        }
    }
}
