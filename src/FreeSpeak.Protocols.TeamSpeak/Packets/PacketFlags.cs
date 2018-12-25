using System;

namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Represents flags set on a packet.
    /// </summary>
    [Flags]
    public enum PacketFlags
    {
        /// <summary>
        /// Denotes that the packet is fragmented.
        /// </summary>
        Fragmented = 0x10,

        /// <summary>
        /// Denotes that the packet is a command.
        /// </summary>
        NewProtocol = 0x20,

        /// <summary>
        /// Denotes that the packet is compressed.
        /// </summary>
        Compressed = 0x40,

        /// <summary>
        /// Denotes that the packet is unencrypted.
        /// </summary>
        Unencrypted = 0x80,
    }
}
