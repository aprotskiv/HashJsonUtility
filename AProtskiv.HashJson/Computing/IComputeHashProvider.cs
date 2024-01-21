using System;

namespace AProtskiv.HashJson.Computing
{
    public interface IComputeHashProvider : IDisposable
    {
        byte[] ComputeHash(string input);

        string ComputeHashAsString(string input);
    }
}