using System;
using System.Globalization;
using System.Text;
using FreeSpeak.Loggers;
using FreeSpeak.PacketProcessing;

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

            var keys = Encryption.GenerateKeys();
            Random rnd = new Random();
            //byte[] nonce = new byte[] { 0x6D, 0x5C, 0x66, 0x69, 0x72, 0x65, 0x77, 0x61, 0x6C, 0x6C, 0x33, 0x32, 0x2E, 0x63, 0x70, 0x6C };
            byte[] siv = new byte[64];
            rnd.NextBytes(siv);
            byte[] meta = new byte[5];
            rnd.NextBytes(meta);
            string value = "HELLO WORLD";
            (byte[] enKey, byte[] enNonce) = Encryption.GenerateKey(Packets.PacketType.Ack, 0, 0, false, siv);
            (byte[] deKey, byte[] deNonce) = Encryption.GenerateKey(Packets.PacketType.Ack, 0, 0, false, siv);
            byte[] unencrypted = Encoding.UTF8.GetBytes(value);
            (byte[] encrypted, byte[] mac) = Encryption.Encrypt(enKey, enNonce, meta, unencrypted);
            byte[] decrypted = Encryption.Decrypt(deKey, deNonce, meta, encrypted, mac);

            logger.WriteWarning($"Encrypted: {Encoding.UTF8.GetString(decrypted)}");

            using TeamSpeakServer server = new TeamSpeakServer(port, logger);
            server.Listen();
        }
    }
}
