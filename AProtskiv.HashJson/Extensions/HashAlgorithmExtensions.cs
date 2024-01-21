using System.Security.Cryptography;
using System.Text;

namespace AProtskiv.HashJson.Extensions
{
    public static class HashAlgorithmExtensions
    {
        public static byte[] ComputeHash(this HashAlgorithm algorithm, string inputString)
        {
            return algorithm.ComputeHash(Encoding.Unicode.GetBytes(inputString));
        }
    }
}