using System;
using FreeSpeak.Servers;

namespace FreeSpeak
{
    /// <summary>
    /// Class logging things to the console.
    /// </summary>
    /// <seealso cref="ILogger" />
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc/>
        public void WriteError(string message)
            => WriteLine($"[ERROR] {message}", ConsoleColor.Red);

        /// <inheritdoc/>
        public void WriteInfo(string message)
            => WriteLine($"[INFO] {message}", ConsoleColor.White);

        /// <inheritdoc/>
        public void WriteWarning(string message)
            => WriteLine($"[WARNING] {message}", ConsoleColor.Yellow);

        private void WriteLine(string text, ConsoleColor color)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
    }
}
