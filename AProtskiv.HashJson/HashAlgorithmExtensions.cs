using System.Security.Cryptography;
using System.Text;

namespace AProtskiv.HashJson
{
	public static class HashAlgorithmExtensions
	{
		public static byte[] ComputeHash(this HashAlgorithm algorithm, string inputString)
		{
			return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		public static string ComputeHashAsString(this HashAlgorithm algorithm, string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in ComputeHash(algorithm, inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
	}
}
