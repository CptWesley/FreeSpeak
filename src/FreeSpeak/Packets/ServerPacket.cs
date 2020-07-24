using System;
using System.IO;
using ExtensionNet;
using FreeSpeak.PacketProcessing;
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

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="bytes">The packet bytes.</param>
        /// <returns>The parsed client packet.</returns>
        public static ServerPacket Parse(byte[] bytes)
        {
            using MemoryStream ms = new MemoryStream(bytes);
            return Parse(ms);
        }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The packet stream.</param>
        /// <returns>The parsed client packet.</returns>
        public static ServerPacket Parse(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            ulong mac = stream.ReadUInt64(Endianness.BigEndian);
            ushort pid = stream.ReadUInt16(Endianness.BigEndian);
            byte typeFlags = stream.ReadUInt8();
            PacketType type = (PacketType)(typeFlags & 0x0F);
            PacketFlags flags = (PacketFlags)(typeFlags & 0xF0);
            PacketData data = PacketData.Parse(type, flags, stream);
            return new ServerPacket(mac, pid, type, flags, data);
        }

        /// <summary>
        /// Converts the packet to bytes that can be transmitted over the network.
        /// </summary>
        /// <returns>The bytes representing the packet.</returns>
        public byte[] ToBytes()
        {
            using MemoryStream ms = new MemoryStream();
            ms.Write(MessageAuthenticationCode, Endianness.BigEndian);
            ms.Write(PacketId, Endianness.BigEndian);
            ms.Write((byte)((byte)Flags + (byte)Type));
            ms.Write(Data.ToBytes());
            return ms.ToArray();
        }

        /// <summary>
        /// Gets the header bytes.
        /// </summary>
        /// <returns>The bytes of the header.</returns>
        public byte[] GetHeader()
            => Encryption.GetHeader(PacketId, Type, Flags);

        public override string ToString()
            => $"MAC={MessageAuthenticationCode} PID={PacketId} Type={Type} Flags={Flags} Data={{{Data}}}";
    }
}
