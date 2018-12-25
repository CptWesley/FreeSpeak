using System;
using System.Diagnostics.CodeAnalysis;

using ExtensionNet.Endian;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Represents a packet send from the client to the server.
    /// </summary>
    public class ClientPacket
    {
        /// <summary>
        /// The maximum data size.
        /// </summary>
        public const int MaxDataSize = 487;

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
        /// Gets or sets the client id.
        /// </summary>
        public ushort ClientId { get; set; }

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
        /// Creates a <see cref="ClientPacket"/> instance from a byte array.
        /// </summary>
        /// <param name="bytes">The bytes to parse.</param>
        /// <returns>An instance parsed from the bytes.</returns>
        public static ClientPacket FromBytes(byte[] bytes)
        {
            ulong mac = BitConverter.ToUInt64(bytes, 0);
            ushort pid = BitConverter.ToUInt16(bytes, 8);
            ushort cid = BitConverter.ToUInt16(bytes, 10);
            byte pt = bytes[12];
            byte[] data = new byte[bytes.Length - 13];
            Array.Copy(bytes, 13, data, 0, data.Length);

            PacketFlags flags = (PacketFlags)(pt & 0xF0);
            PacketType type = (PacketType)(pt & 0x0F);

            if (BitConverter.IsLittleEndian)
            {
                mac = mac.ChangeEndianness();
                pid = pid.ChangeEndianness();
                cid = cid.ChangeEndianness();
            }

            return new ClientPacket()
            {
                MessageAuthenticationCode = mac,
                PacketId = pid,
                ClientId = cid,
                Flags = flags,
                Type = type,
                Data = data
            };
        }
    }
}
