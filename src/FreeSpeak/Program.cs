using System;
using FreeSpeak.Servers.TeamSpeak;

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
            ConsoleLogger logger = new ConsoleLogger();
            TeamSpeakServer server = new TeamSpeakServer(439);
            server.Logger = logger;
            server.StartListening();

            while (true)
            {
                string command = Console.ReadLine();
                switch (command)
                {
                    case "exit":
                        return;
                    default:
                        logger.WriteError($"Unknown command '{command}'.");
                        break;
                }
            }
        }
    }
}
