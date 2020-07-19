using System.Globalization;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;

namespace FreeSpeak
{
    /// <summary>
    /// Entry point of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public static void Main(string[] args)
        {
            int port = args?.Length > 0 ? int.Parse(args[0], CultureInfo.InvariantCulture) : 9987;
            using TeamSpeakServer udp = new TeamSpeakServer(port);
            ILogger logger = new ConsoleLogger();

            while (true)
            {
                ClientPacket packet = udp.Receive();
                logger.WriteInfo($"{packet.Sender.Address}:{packet.Sender.Port} -> {packet.ClientId} {packet.PacketId} {packet.Type} {packet.Flags}");
            }
        }
    }
}
