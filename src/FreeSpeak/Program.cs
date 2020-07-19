using FreeSpeak.Loggers;
using FreeSpeak.Packets;
using System.Net;
using System.Net.Sockets;

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
            using UdpClient udp = new UdpClient(9987);
            ILogger logger = new ConsoleLogger();

            while (true)
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] packetBytes = udp.Receive(ref ep);
                ClientPacket packet = ClientPacket.Parse(packetBytes);
                logger.WriteInfo($"{ep.Address}:{ep.Port} -> {packet.ClientId} {packet.PacketId} {packet.Type} {packet.Flags}");
            }
        }
    }
}
