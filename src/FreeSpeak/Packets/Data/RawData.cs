using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents raw data, used when the data is fragmented, encrypted or compressed.
    /// </summary>
    /// <seealso cref="PacketData" />
    public class RawData : PacketData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawData"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public RawData(byte[] data)
            => Data = data;

        /// <summary>
        /// Gets the data.
        /// </summary>
        public IEnumerable<byte> Data { get; }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The data found in the stream.</returns>
        public static RawData Parse(Stream stream)
            => new RawData(stream.ReadAllBytes());

        /// <inheritdoc/>
        public override byte[] ToBytes()
            => Data.ToArray();
    }
}
