using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExtensionNet.Endian;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Represents a packet send from the server to a client.
    /// </summary>
    public class ServerPacket
    {
        /// <summary>
        /// The maximum data size.
        /// </summary>
        public const int MaxDataSize = 489;

        private byte[] _data = Array.Empty<byte>();

        /// <summary>
        /// Gets or sets the EAX Message Authentication Code.
        /// </summary>
        public ulong MessageAuthenticationCode { get; set; }

        /// <summary>
        /// Gets or sets the packet id.
        /// </summary>
        public ushort PacketId { get; set; }

        /// <summary>
        /// Gets or sets the packet flags.
        /// </summary>
        public PacketFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets the packet type.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "We don't mind changing the entire array.")]
        public byte[] Data
        {
            get => _data;
            set
            {
                if (value.Length > MaxDataSize)
                {
                    throw new ArgumentException($"Maximum data length is {MaxDataSize}, but tried to set {value.Length} bytes.");
                }

                _data = value;
            }
        }

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

            ulong mac = BitConverter.ToUInt64(bytes, 0);
            ushort pid = BitConverter.ToUInt16(bytes, 8);
            byte pt = bytes[10];
            byte[] data = new byte[bytes.Length - 11];
            Array.Copy(bytes, 11, data, 0, data.Length);

            PacketFlags flags = (PacketFlags)(pt & 0xF0);
            PacketType type = (PacketType)(pt & 0x0F);

            if (BitConverter.IsLittleEndian)
            {
                mac = mac.ChangeEndianness();
                pid = pid.ChangeEndianness();
            }

            return new ServerPacket()
            {
                MessageAuthenticationCode = mac,
                PacketId = pid,
                Flags = flags,
                Type = type,
                Data = data
            };
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

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ServerPacket other)
            {
                return MessageAuthenticationCode == other.MessageAuthenticationCode
                    && PacketId == other.PacketId
                    && Flags == other.Flags
                    && Type == other.Type
                    && _data.SequenceEqual(other._data);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int mac1 = (int)(MessageAuthenticationCode & 0x00000000FFFFFFFF);
            int mac2 = (int)(MessageAuthenticationCode & 0xFFFFFFFF00000000);
            int mac = mac1 + mac2;

            return GetType().GetHashCode() * (mac + (2 * PacketId) + (3 * ((int)Flags + (int)Type)) + (4 * _data.Sum(x => x)));
        }
    }
}
