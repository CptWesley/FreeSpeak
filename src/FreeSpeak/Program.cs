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
            using Server udp = new Server(9987);
            ILogger logger = new ConsoleLogger();

            while (true)
            {
                ClientPacket packet = udp.Receive();
                logger.WriteInfo($"{packet.Sender.Address}:{packet.Sender.Port} -> {packet.ClientId} {packet.PacketId} {packet.Type} {packet.Flags}");
            }
        }
    }
}
