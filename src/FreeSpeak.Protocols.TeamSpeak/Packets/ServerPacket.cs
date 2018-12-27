using System;
using System.IO;
using ExtensionNet.Endian;
using ExtensionNet.Streams;
using ExtensionNet.Types;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Represents a packet send from the server to a client.
    /// </summary>
    public class ServerPacket : Packet
    {
        /// <inheritdoc />
        public override int MaxDataSize => 489;

        /// <summary>
        /// Creates a <see cref="ServerPacket"/> instance from a byte array.
        /// </summary>
        /// <param name="bytes">The bytes to parse.</param>
        /// <returns>An instance parsed from the bytes.</returns>
        public static ServerPacket FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            else if (bytes.Length < 11)
            {
                throw new ArgumentException($"The packet must be at least 11 bytes, but was only {bytes.Length} bytes.");
            }

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                ulong mac = stream.ReadUInt64(Endianness.BigEndian);
                ushort pid = stream.ReadUInt16(Endianness.BigEndian);
                byte pt = stream.ReadUInt8();
                byte[] data = stream.ReadUInt8(bytes.Length - 11);

                PacketFlags flags = (PacketFlags)(pt & 0xF0);
                PacketType type = (PacketType)(pt & 0x0F);

                return new ServerPacket()
                {
                    MessageAuthenticationCode = mac,
                    PacketId = pid,
                    Flags = flags,
                    Type = type,
                    Data = data
                };
            }
        }

        /// <summary>
        /// Converts the object to a series of bytes.
        /// </summary>
        /// <param name="packet">The packet to convert.</param>
        /// <returns>Bytes representing the packet.</returns>
        public static byte[] ToBytes(ServerPacket packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            return packet.ToBytes();
        }

        /// <summary>
        /// Converts the packet to a series of bytes.
        /// </summary>
        /// <returns>Bytes representing the packet.</returns>
        public byte[] ToBytes()
        {
            int length = Data.Length + 11;
            byte[] result = new byte[length];

            byte[] bytes;
            bytes = BitConverter.GetBytes(BitConverter.IsLittleEndian ? MessageAuthenticationCode.ChangeEndianness() : MessageAuthenticationCode);
            Array.Copy(bytes, 0, result, 0, bytes.Length);
            bytes = BitConverter.GetBytes(BitConverter.IsLittleEndian ? PacketId.ChangeEndianness() : PacketId);
            Array.Copy(bytes, 0, result, 8, bytes.Length);
            result[10] = (byte)((byte)Flags + (byte)Type);
            Array.Copy(Data, 0, result, 11, Data.Length);

            return result;
        }
    }
}
