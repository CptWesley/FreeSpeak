using System;

namespace FreeSpeak.Packets
{
    /// <summary>
    /// The different packet flags.
    /// </summary>
    [Flags]
    public enum PacketFlags
    {
        /// <summary>
        /// Indicates that a packet has no flags.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Indicates a packet is unencrypted.
        /// </summary>
        Unencrypted = 0x80,

        /// <summary>
        /// Indicates a packet is compressed.
        /// </summary>
        Compressed = 0x40,

        /// <summary>
        /// Indicates a packet follows the new protocol.
        /// </summary>
        NewProtocol = 0x20,

        /// <summary>
        /// Indicates a packet is fragmented.
        /// </summary>
        Fragmented = 0x10,
    }
}
