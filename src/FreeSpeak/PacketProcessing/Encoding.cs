using System;

namespace FreeSpeak.PacketProcessing
{
    /// <summary>
    /// Provides methods to deal with input that are related to the encoding.
    /// </summary>
    public static class Encoding
    {
        /// <summary>
        /// Gets the bytes of the UTF8 input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The bytes of the input string.</returns>
        public static byte[] Utf8(string input)
            => System.Text.Encoding.UTF8.GetBytes(input);

        /// <summary>
        /// Gets the UTF8 string of the input bytes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The string of the input bytes.</returns>
        public static string Utf8(byte[] input)
            => System.Text.Encoding.UTF8.GetString(input);

        /// <summary>
        /// Converts the given base64 string to bytes.
        /// </summary>
        /// <param name="input">The base64 input.</param>
        /// <returns>The bytes.</returns>
        public static byte[] Base64(string input)
            => Convert.FromBase64String(input?.Replace("\\/", "/", StringComparison.InvariantCulture));

        /// <summary>
        /// Converts the given bytes to a base64 string.
        /// </summary>
        /// <param name="input">The bytes.</param>
        /// <returns>The base64 string.</returns>
        public static string Base64(byte[] input)
            => Convert.ToBase64String(input).Replace("/", "\\/", StringComparison.InvariantCulture);
    }
}
