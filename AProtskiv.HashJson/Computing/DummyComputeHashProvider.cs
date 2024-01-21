namespace AProtskiv.HashJson.Computing
{
    public class DummyComputeHashProvider : IComputeHashProvider
    {
        public byte[] ComputeHash(string input)
        {
            throw new System.NotImplementedException();
        }

        public string ComputeHashAsString(string input) => input;

        public void Dispose()
        {
        }
    }
}