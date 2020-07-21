using System;

namespace FreeSpeak.Loggers
{
    /// <summary>
    /// Class logging things to the console.
    /// </summary>
    /// <seealso cref="ILogger" />
    public class ConsoleLogger : Logger
    {
        private static readonly object Lock = new object();

        /// <inheritdoc/>
        public override void WriteError(string message)
            => WriteLine($"[ERROR] {message}", ConsoleColor.Red);

        /// <inheritdoc/>
        public override void WriteInfo(string message)
            => WriteLine($"[INFO] {message}", ConsoleColor.White);

        /// <inheritdoc/>
        public override void WriteWarning(string message)
            => WriteLine($"[WARNING] {message}", ConsoleColor.Yellow);

        private static void WriteLine(string text, ConsoleColor color)
        {
            lock (Lock)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
        }
    }
}
