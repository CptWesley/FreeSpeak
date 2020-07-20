using System;
using System.IO;
using ExtensionNet;
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
            using MemoryStream ms = new MemoryStream();
            ms.Write(MessageAuthenticationCode, Endianness.BigEndian);
            ms.Write(PacketId, Endianness.BigEndian);
            ms.Write((byte)((byte)Flags + (byte)Type));
            ms.Write(Data.ToBytes());
            return ms.ToArray();
        }
    }
}
