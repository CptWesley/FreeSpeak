using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Data for acknowledgement packets.
    /// </summary>
    /// <seealso cref="PacketData" />
    public class AckData : PacketData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AckData"/> class.
        /// </summary>
        /// <param name="packetId">The packet identifier.</param>
        public AckData(ushort packetId)
            => PacketId = packetId;

        /// <summary>
        /// Gets the packet identifier.
        /// </summary>
        public ushort PacketId { get; }

        /// <inheritdoc/>
        public override byte[] ToBytes()
            => PacketId.GetBytes(Endianness.BigEndian);
    }
}
