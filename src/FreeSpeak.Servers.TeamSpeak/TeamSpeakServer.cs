using System.Collections.Generic;
using System.Net;
using FreeSpeak.Protocols.TeamSpeak.Packets;

namespace FreeSpeak.Servers.TeamSpeak
{
    /// <summary>
    /// Represents a TeamSpeak server.
    /// </summary>
    /// <seealso cref="Server" />
    public class TeamSpeakServer : Server
    {
        private Dictionary<IPEndPoint, ClientIdentity> identities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSpeakServer"/> class.
        /// </summary>
        /// <param name="port">The port on which the server is listening.</param>
        public TeamSpeakServer(int port)
            : base(port)
        {
            identities = new Dictionary<IPEndPoint, ClientIdentity>();
        }

        /// <inheritdoc/>
        protected override void MessageReceivedCallback(IPEndPoint ep, byte[] message)
        {
            if (!identities.ContainsKey(ep))
            {
                Logger.WriteInfo($"New connection from {ep}");
                identities.Add(ep, new ClientIdentity(ep));
            }

            ClientIdentity identity = identities[ep];
            identity.Handle(ClientPacket.FromBytes(message));
        }
    }
}
