using System;
using System.Globalization;
using ExtensionNet.Endian;
using FreeSpeak.Loggers;
using FreeSpeak.Packets;
using FreeSpeak.Packets.Data;

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
            ILogger logger = new ConsoleLogger();
            using TeamSpeakServer server = new TeamSpeakServer(port, logger);

            while (true)
            {
                ClientPacket packet = server.Receive();

                if (packet.Type == PacketType.Init1)
                {
                    ulong mac = 0x545333494E495431;
                    if (((ClientHandshakeData)packet.Data).Step == 0)
                    {
                        Handshake0Data data = packet.Data as Handshake0Data;
                        ServerPacket sp = new ServerPacket(mac, 101, PacketType.Init1, PacketFlags.Unencrypted, new Handshake1Data(0, 0, data.Random.ChangeEndianness()));
                        Console.Write($"Send rnd: {data.Random.ChangeEndianness()}");
                        server.Send(packet.Sender, sp);
                    }
                    else if (((ClientHandshakeData)packet.Data).Step == 2)
                    {
                        Handshake2Data data = packet.Data as Handshake2Data;
                        ServerPacket sp = new ServerPacket(mac, 101, PacketType.Init1, PacketFlags.Unencrypted, new Handshake3Data(4, 7, 10, new byte[100]));
                        server.Send(packet.Sender, sp);
                    }
                }
            }
        }
    }
}
