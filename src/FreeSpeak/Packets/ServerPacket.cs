using System;
using ExtensionNet.Endian;
using FreeSpeak.Packets.Data;

namespace FreeSpeak.Packets
{
    /// <summary>
    /// Represents a packet send by the server.
    /// </summary>
    public class ServerPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPacket"/> class.
        /// </summary>
        /// <param name="mac">The mac.</param>
        /// <param name="pid">The pid.</param>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="data">The data.</param>
        public ServerPacket(ulong mac, ushort pid, PacketType type, PacketFlags flags, PacketData data)
        {
            MessageAuthenticationCode = mac;
            PacketId = pid;
            Type = type;
            Flags = flags;
            Data = data;
        }

        /// <summary>
        /// Gets the message authentication code.
        /// </summary>
        public ulong MessageAuthenticationCode { get; }

        /// <summary>
        /// Gets the packet identifier.
        /// </summary>
        public ushort PacketId { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public PacketType Type { get; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        public PacketFlags Flags { get; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public PacketData Data { get; }

        public byte[] ToBytes()
        {
            byte[] data = Data.ToBytes();
            byte[] result = new byte[11 + data.Length];
            byte[] mac = BitConverter.IsLittleEndian ? BitConverter.GetBytes(MessageAuthenticationCode.ChangeEndianness()) : BitConverter.GetBytes(MessageAuthenticationCode);
            Array.Copy(mac, 0, result, 0, 8);
            byte[] pid = BitConverter.IsLittleEndian ? BitConverter.GetBytes(PacketId.ChangeEndianness()) : BitConverter.GetBytes(PacketId);
            Array.Copy(pid, 0, result, 8, 2);
            result[10] = (byte)((byte)Flags + (byte)Type);
            Array.Copy(data, 0, result, 11, data.Length);
            return result;
        }
    }
}
