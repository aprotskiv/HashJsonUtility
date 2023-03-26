using System.Collections.Generic;

namespace AProtskiv.HashJson.Reserved
{
    public class ReservedNamespaceDictionary : Dictionary<string, ReservedNamespace>, IReservedNamespaceDictionary
    {
        public string Separator { get; } = ":";
    }
}
