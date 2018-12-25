namespace FreeSpeak.Protocols.TeamSpeak.Packets
{
    /// <summary>
    /// Denotes the type of a packet.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Denotes a voice packet.
        /// </summary>
        Voice = 0x00,

        /// <summary>
        /// Denotes a whisper packet.
        /// </summary>
        VoiceWhisper = 0x01,

        /// <summary>
        /// Denotes a command packet.
        /// </summary>
        Command = 0x02,

        /// <summary>
        /// Denotes a command packet.
        /// </summary>
        CommandLow = 0x03,

        /// <summary>
        /// Denotes a ping packet.
        /// </summary>
        Ping = 0x04,

        /// <summary>
        /// Denotes a ping answer packet.
        /// </summary>
        Pong = 0x05,

        /// <summary>
        /// Denotes an acknowledgement packet.
        /// </summary>
        Acknowledge = 0x06,

        /// <summary>
        /// Denotes an acknowledgement packet.
        /// </summary>
        AcknowledgeLow = 0x07,

        /// <summary>
        /// Denotes an initialization packet.
        /// </summary>
        Init = 0x08
    }
}
