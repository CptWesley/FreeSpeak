using System.IO;
using System.Text;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents the data in a command (low) packet.
    /// </summary>
    /// <seealso cref="PacketData" />
    public class CommandData : PacketData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandData(string command)
            => Command = command;

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Parses the specified stream as command data.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The parsed command data.</returns>
        public static CommandData Parse(Stream stream)
        {
            using StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            return new CommandData(sr.ReadToEnd());
        }

        /// <inheritdoc/>
        public override byte[] ToBytes()
            => Encoding.UTF8.GetBytes(Command);
    }
}
