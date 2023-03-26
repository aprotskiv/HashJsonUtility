using System.Collections.Generic;

namespace AProtskiv.HashJson.Reserved
{
    public interface IReservedNamespaceDictionary : IReadOnlyDictionary<string, ReservedNamespace>
    {
        string Separator { get; }
    }
}
