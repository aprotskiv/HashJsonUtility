using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AProtskiv.HashJson.Extensions
{
    public static class ByteArrayEntensions            
    {
        public static string ToHexadecimal(this IReadOnlyList<byte> byteArray)
        {
            var sb = new StringBuilder();

            foreach (byte b in byteArray)
                 sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] StringToByteArray(String hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new ArgumentException("Parameter must be a Hexadecimal string", nameof(hex));
            }

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
