namespace FreeSpeak.Loggers
{
    /// <summary>
    /// Logger abstract class.
    /// </summary>
    /// <seealso cref="ILogger" />
    public abstract class Logger : ILogger
    {
        /// <inheritdoc/>
        public abstract void WriteError(string message);

        /// <inheritdoc/>
        public abstract void WriteInfo(string message);

        /// <inheritdoc/>
        public abstract void WriteWarning(string message);

        /// <inheritdoc/>
        public void WriteError(object message)
            => WriteError(message?.ToString());

        /// <inheritdoc/>
        public void WriteInfo(object message)
            => WriteInfo(message?.ToString());

        /// <inheritdoc/>
        public void WriteWarning(object message)
            => WriteWarning(message?.ToString());
    }
}
