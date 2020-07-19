namespace FreeSpeak.Loggers
{
    /// <summary>
    /// Interface for logging mechanisms.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void WriteInfo(string message);

        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void WriteWarning(string message);

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void WriteError(string message);
    }
}