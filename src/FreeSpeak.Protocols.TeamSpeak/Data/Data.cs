using ExtensionNet.Reflective;

namespace FreeSpeak.Protocols.TeamSpeak.Data
{
    /// <summary>
    /// Abstract class for data pieces of packets.
    /// </summary>
    public abstract class Data
    {
        /// <summary>
        /// Converts the data into a series of bytes.
        /// </summary>
        /// <returns>The data is byte form.</returns>
        public abstract byte[] ToBytes();

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => this.InternallyEquals(obj, true);

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.GetInternalHashCode(true);
    }
}
