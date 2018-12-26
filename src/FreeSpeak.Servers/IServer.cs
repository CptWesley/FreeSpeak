namespace FreeSpeak.Servers
{
    /// <summary>
    /// Interface for FreeSpeak servers.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Gets or sets the logger used for logging messages.
        /// </summary>
        ILogger Logger { get; set; }
    }
}
