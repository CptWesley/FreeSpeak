using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FreeSpeak.Protocols.TeamSpeak.Compression
{
    /// <summary>
    /// Class used for compressing data. Implementation based on: https://github.com/ReSpeak/quicklz
    /// </summary>
    public class Compressor
    {
        private const int HashtableSize = 4096;

        /// <summary>
        /// Compresses the specified data.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        public byte[] Compress(byte[] data)
        {
            //int headerLength = data.Length < 216 ? 3 : 9;
            int headerLength = 9;
            List<byte> result = new List<byte>();
            result.AddRange(new byte[headerLength + 4]);

            int control = 1 << 31;
            int controlPosition = headerLength;
            int sourcePosition = 0;

            int[] hashTable = new int[HashtableSize];
            int[] cacheTable = new int[HashtableSize];
            BitVector32 hashCounter = default(BitVector32);
            int lits = 0;

            while (sourcePosition + 10 < data.Length)
            {
                byte[] nextBytes = new byte[] { 0, data[sourcePosition], data[sourcePosition + 1], data[sourcePosition + 2] };
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(nextBytes);
                }

                int next = BitConverter.ToInt32(nextBytes, 0);
                int hash = Hash(next);
                int offset = hashTable[hash];
                int cache = cacheTable[hash];
                bool counter = hashCounter[hash];
                cacheTable[hash] = next;
                hashTable[hash] = sourcePosition;

                if (cache == next && counter && (sourcePosition - offset >= 3
                    || (sourcePosition == offset + 1 && lits >= 3 && sourcePosition > 3 && SameValues(data, sourcePosition - 3, sourcePosition + 3))))
                {
                    control = (control >> 1) | (1 << 31);
                    int matchLength = 3;
                    int remainder = Math.Min(data.Length - 4 - sourcePosition, 0xff);
                    while (data[offset + matchLength] == data[sourcePosition + matchLength] && matchLength < remainder)
                    {
                        matchLength++;
                    }

                    if (matchLength < 18)
                    {
                        byte[] bytes = BitConverter.GetBytes((ushort)(hash << 4 | (matchLength - 2)));
                        if (!BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(bytes);
                        }

                        result.AddRange(bytes);
                    }
                    else
                    {
                        byte[] bytes = BitConverter.GetBytes((uint)(hash << 4 | (matchLength << 16)));
                        if (!BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(bytes);
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            result.Add(bytes[i]);
                        }
                    }

                    sourcePosition += matchLength;
                    lits = 0;
                }
                else
                {
                    lits++;
                    hashCounter[hash] = true;
                    result.Add(data[sourcePosition]);
                    sourcePosition++;
                    control >>= 1;
                }
            }

            while (sourcePosition < data.Length)
            {
                if ((control & 1) != 0)
                {
                    WriteControl(result, controlPosition, (control >> 1) | (1 << 31));
                    controlPosition = result.Count;
                    result.AddRange(new byte[4]);
                    control = 1 << 31;
                }

                result.Add(data[sourcePosition]);
                sourcePosition++;
                control >>= 1;
            }

            while ((control & 1) == 0)
            {
                control >>= 1;
            }

            WriteControl(result, controlPosition, (control >> 1) | (1 << 31));
            //throw new Exception($"Size: {data.Length}\nHeader length: {headerLength}");
            WriteHeader(result, data.Length, headerLength == 3);

            return result.ToArray();
        }

        private void WriteControl(List<byte> result, int position, int control)
        {
            byte[] bytes = BitConverter.GetBytes(control);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            for (int i = 0; i < 4; i++)
            {
                result[position + i] = bytes[i];
            }
        }

        private void WriteHeader(List<byte> result, int sourceLength, bool shortHeader)
        {
            byte flags = 0x45;

            if (shortHeader)
            {
                result[0] = flags;
                result[1] = (byte)result.Count;
                result[2] = (byte)sourceLength;
            }
            else
            {
                result[0] = (byte)(flags | 0x02);

                byte[] rLengthBytes = BitConverter.GetBytes(result.Count);
                byte[] sLengthBytes = BitConverter.GetBytes(sourceLength);

                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(rLengthBytes);
                    Array.Reverse(sLengthBytes);
                }

                for (int i = 0; i < 4; i++)
                {
                    result[1 + i] = rLengthBytes[i];
                }

                for (int i = 0; i < 4; i++)
                {
                    result[5 + i] = sLengthBytes[i];
                }
            }
        }

        private int Hash(int value) => ((value >> 12) ^ value) & 0xfff;

        private bool SameValues(byte[] data, int start, int end)
        {
            int value = data[start];

            for (int i = 1; i < end - start + 1; i++)
            {
                if (data[i] != value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
