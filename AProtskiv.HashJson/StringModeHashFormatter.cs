using AProtskiv.HashJson.Extensions;
using System;

namespace AProtskiv.HashJson
{
    public class StringModeHashFormatter : IHashFormatter
    {
        private readonly HashFormattingMode _mode;

        public StringModeHashFormatter(HashFormattingMode mode)
        {
            _mode = mode;
        }

        public string Format(byte[] hash)
        {
            return _mode switch
            {
                HashFormattingMode.Base64 => System.Convert.ToBase64String(hash),
                HashFormattingMode.Hexadecimal => hash.ToHexadecimal(),
                _ => throw new InvalidOperationException($"Unsupported hashing mode '{_mode}'"),
            };
        }
    }
}