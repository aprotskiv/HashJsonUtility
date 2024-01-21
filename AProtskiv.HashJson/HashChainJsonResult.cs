namespace AProtskiv.HashJson
{
    public sealed class HashChainJsonResult
    {
        public string HashedDocument { get; set; }
        public byte[] HashChainOfSalt { get; set; }
    }
}