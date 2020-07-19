namespace FreeSpeak.Packets
{
    /// <summary>
    /// The different packet types.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Indicates that a packet is a voice packet.
        /// </summary>
        Voice = 0x00,

        /// <summary>
        /// Indicates that a packet is a whisper packet.
        /// </summary>
        VoiceWhisper = 0x01,

        /// <summary>
        /// Indicates that a packet is a command packet.
        /// </summary>
        Command = 0x02,

        /// <summary>
        /// Indicates that a packet is a command low packet.
        /// </summary>
        CommandLow = 0x03,

        /// <summary>
        /// Indicates that a packet is a ping packet.
        /// </summary>
        Ping = 0x04,

        /// <summary>
        /// Indicates that a packet is a pong packet.
        /// </summary>
        Pong = 0x05,

        /// <summary>
        /// Indicates that a packet is a acknowledgement packet.
        /// </summary>
        Ack = 0x06,

        /// <summary>
        /// Indicates that a packet is a acknowledgement low packet.
        /// </summary>
        AckLow = 0x07,

        /// <summary>
        /// Indicates that a packet is a init packet.
        /// </summary>
        Init1 = 0x08,
    }
}
