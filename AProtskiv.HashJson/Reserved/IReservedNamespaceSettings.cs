using System.Collections.Generic;

namespace AProtskiv.HashJson.Reserved
{
    public interface IReservedNamespaceSettings
    {
        Dictionary<string, ReservedNamespace> Items { get; }

        string Separator { get; }
    }
}
