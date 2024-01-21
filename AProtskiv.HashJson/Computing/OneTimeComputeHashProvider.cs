using AProtskiv.HashJson.Extensions;
using System.Security.Cryptography;

namespace AProtskiv.HashJson.Computing
{
    public class OneTimeComputeHashProvider : IComputeHashProvider
    {
        private readonly HashAlgorithm _algorithm;
        private readonly IHashFormatter _formatter;

        public OneTimeComputeHashProvider(HashAlgorithm algorithm, IHashFormatter formatter)
        {
            _algorithm = algorithm;
            _formatter = formatter;
        }

        public byte[] ComputeHash(string input)
        {
            return _algorithm.ComputeHash(input);
        }

        public string ComputeHashAsString(string input)
        {
            var hash = this.ComputeHash(input);
            return _formatter.Format(hash);
        }

        #region Disposing

        ~OneTimeComputeHashProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool isDispose)
        {
            if (isDispose)
            {
                // release managed resources
                _algorithm?.Dispose();
            }
        }

       

        #endregion
    }
}