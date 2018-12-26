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
            using (TeamSpeakServer server = new TeamSpeakServer(439))
            {
                server.Logger = new ConsoleLogger();
                server.Start();
            }
        }
    }
}
