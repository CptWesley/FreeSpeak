namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents packet data.
    /// </summary>
    public abstract class PacketData
    {
        /// <summary>
        /// Converts the packet data to bytes.
        /// </summary>
        /// <returns>The packet data as bytes.</returns>
        public abstract byte[] ToBytes();
    }
}
