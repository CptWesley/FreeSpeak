using System.IO;
using ExtensionNet;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents packet data.
    /// </summary>
    public abstract class PacketData
    {
        /// <summary>
        /// Parses data from a stream.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The packet data from the stream.</returns>
        public static PacketData Parse(PacketType type, PacketFlags flags, Stream stream)
        {
            if (flags.HasFlag(PacketFlags.Compressed) || !flags.HasFlag(PacketFlags.Unencrypted) || flags.HasFlag(PacketFlags.Fragmented))
            {
                return RawData.Parse(stream);
            }

            return type switch
            {
                PacketType.Ack => new AckData(stream.ReadUInt16(Endianness.BigEndian)),
                PacketType.Init1 => HandshakeData.Parse(stream),
                PacketType.Command => CommandData.Parse(stream),
                PacketType.Ping => new PingData(),
                _ => RawData.Parse(stream),
            };
        }

        /// <summary>
        /// Converts the packet data to bytes.
        /// </summary>
        /// <returns>The packet data as bytes.</returns>
        public abstract byte[] ToBytes();
    }
}
