namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Indicates that the data belongs to a handshake packet.
    /// </summary>
    public abstract class HandshakeData : PacketData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeData"/> class.
        /// </summary>
        /// <param name="step">The step.</param>
        public HandshakeData(byte step)
        {
            Step = step;
        }

        /// <summary>
        /// Gets the step of the handshake.
        /// </summary>
        public byte Step { get; }
    }
}
