using AProtskiv.HashJson.Extensions;
using System.Security.Cryptography;

namespace AProtskiv.HashJson.Computing
{
    /// <summary>
    /// Applies Hash function many times (HashChain length)
    /// </summary>
    public class ChainComputeHashProvider : IComputeHashProvider
    {
        private readonly HashAlgorithm _algorithm;
        private readonly int _hashChainLength;
        private readonly IHashFormatter _formatter;

        public ChainComputeHashProvider(HashAlgorithm algorithm, int hashChainLength, IHashFormatter formatter)
        {
            _algorithm = algorithm;
            _hashChainLength = hashChainLength;
            _formatter = formatter;
        }

        public byte[] ComputeHash(string input)
        {
            var hash = _algorithm.ComputeHash(input); // compute first time

            var remainigTimes = _hashChainLength - 1; // compute [hashChainLength - 1] times
            while (remainigTimes > 0)
            {
                hash = _algorithm.ComputeHash(hash);
                remainigTimes--;
            }
            return hash;
        }

        public string ComputeHashAsString(string input)
        {
            var hash = this.ComputeHash(input);
            return _formatter.Format(hash);
        }

        #region Disposing

        ~ChainComputeHashProvider()
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