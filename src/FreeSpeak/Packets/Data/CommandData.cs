using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using ExtensionNet;
using Warpstone;

using static Warpstone.Parsers.BasicParsers;

namespace FreeSpeak.Packets.Data
{
    /// <summary>
    /// Represents the data in a command (low) packet.
    /// </summary>
    /// <seealso cref="PacketData" />
    public class CommandData : PacketData
    {
        private static readonly Parser<string> Identifier = Regex("[a-zA-Z0-9]+");
        private static readonly Parser<string> Value = Regex("[\\S]*");
        private static readonly Parser<(string, string)> Pair = Identifier.ThenSkip(Char('=')).ThenAdd(Value);
        private static readonly Parser<(string, string)> Single = Identifier.Transform(x => (x, (string)null));
        private static readonly Parser<IEnumerable<(string, string)>> Pairs = Many(Or(Pair, Single), Char(' '));
        private static readonly Parser<CommandData> CommandAttributes = Or(
            Identifier.ThenSkip(Char(' ')).ThenAdd(Pairs).Transform((n, a) => new CommandData(n, a.ToDictionary(p => p.Item1, p => p.Item2))),
            Identifier.Transform(n => new CommandData(n, new Dictionary<string, string>()))).ThenEnd();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="attributes">The attributes.</param>
        public CommandData(string command, IDictionary<string, string> attributes)
        {
            Command = command;
            Attributes = attributes.ToImmutableDictionary();
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ImmutableDictionary<string, string> Attributes { get; }

        /// <summary>
        /// Parses the specified stream as command data.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The parsed command data.</returns>
        public static CommandData Parse(Stream stream)
            => Parse(stream.ReadString());

        /// <summary>
        /// Parses the specified string as command data.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The parsed command data.</returns>
        public static CommandData Parse(string command)
            => CommandAttributes.Parse(command);

        /// <inheritdoc/>
        public override byte[] ToBytes()
            => Encoding.UTF8.GetBytes(ToString());

        /// <inheritdoc/>
        public override string ToString()
            => $"{Command} {string.Join(" ", Attributes.Select(p => p.Key + (p.Value == null ? string.Empty : $"={p.Value}")))}".Trim();
    }
}
