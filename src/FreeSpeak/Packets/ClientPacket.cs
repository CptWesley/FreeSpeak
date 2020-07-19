using System;
using System.IO;
using System.Net;
using ExtensionNet.Streams;
using ExtensionNet.Types;

namespace FreeSpeak.Packets
{
    /// <summary>
    /// Represents a client packet.
    /// </summary>
    public class ClientPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPacket"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mac">The mac.</param>
        /// <param name="pid">The pid.</param>
        /// <param name="cid">The cid.</param>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="data">The data.</param>
        public ClientPacket(IPEndPoint sender, ulong mac, ushort pid, ushort cid, PacketType type, PacketFlags flags, byte[] data)
        {
            Sender = sender;
            MessageAuthenticationCode = mac;
            PacketId = pid;
            ClientId = cid;
            Type = type;
            Flags = flags;
            Data = data;
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        public IPEndPoint Sender { get; }

        /// <summary>
        /// Gets the message authentication code.
        /// </summary>
        public ulong MessageAuthenticationCode { get; }

        /// <summary>
        /// Gets the packet identifier.
        /// </summary>
        public ushort PacketId { get; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        public ushort ClientId { get; }

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
        public byte[] Data { get; }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="bytes">The packet bytes.</param>
        /// <returns>The parsed client packet.</returns>
        public static ClientPacket Parse(IPEndPoint sender, byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Parse(sender, ms);
            }
        }

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="stream">The packet stream.</param>
        /// <returns>The parsed client packet.</returns>
        public static ClientPacket Parse(IPEndPoint sender, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            ulong mac = stream.ReadUInt64();
            ushort pid = stream.ReadUInt16(Endianness.BigEndian);
            ushort cid = stream.ReadUInt16(Endianness.BigEndian);
            byte typeFlags = stream.ReadUInt8();
            PacketType type = (PacketType)(typeFlags & 0x0F);
            PacketFlags flags = (PacketFlags)(typeFlags & 0xF0);
            byte[] buffer = new byte[487];
            int size = stream.Read(buffer);
            byte[] data = new byte[size];
            Array.Copy(buffer, data, size);

            return new ClientPacket(sender, mac, pid, cid, type, flags, data);
        }
    }
}
