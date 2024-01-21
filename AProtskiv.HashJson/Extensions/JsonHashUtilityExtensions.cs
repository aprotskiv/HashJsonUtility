using AProtskiv.HashJson.Computing;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AProtskiv.HashJson.Extensions
{
    public static class JsonHashUtilityExtensions
    {
        private static string HashWithAlgorithm(JsonHashUtility utility,
           string inputJson,
           Formatting formatting,
           HashFormattingMode mode,
           KeyedHashAlgorithm algorithm)
        {
            var computehashProvider = new OneTimeComputeHashProvider(algorithm, new StringModeHashFormatter(mode));
            return utility.Hash(inputJson, computehashProvider, formatting);
        }

        public static string HashMD5(this JsonHashUtility utility, string inputJson, Formatting formatting, string salt, HashFormattingMode mode)
        {
            var algorithm = new HMACMD5()
                .WithSalt(salt);

            return HashWithAlgorithm(utility, inputJson, formatting, mode, algorithm);
        }

        public static string HashSHA256(this JsonHashUtility utility, string inputJson, Formatting formatting, string salt, HashFormattingMode mode)
        {
            var algorithm = new HMACSHA256()
                .WithSalt(salt);

            return HashWithAlgorithm(utility, inputJson, formatting, mode, algorithm);
        }

        public static string HashSHA512(this JsonHashUtility utility, string inputJson, Formatting formatting, string salt, HashFormattingMode mode)
        {
            var algorithm = new HMACSHA512()
                .WithSalt(salt);

            return HashWithAlgorithm(utility, inputJson, formatting, mode, algorithm);
        }
    }
}