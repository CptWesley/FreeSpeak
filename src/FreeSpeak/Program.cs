using System.Globalization;
using FreeSpeak.Loggers;

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
            //using TeamSpeakServer server = new TeamSpeakServer(port, logger);
            //server.Listen();
            new RelayServer("127.0.0.1", 9987, 19987, logger).Listen();
        }
    }
}
