using System.Collections.Generic;

namespace AProtskiv.HashJson.Reserved
{

    public class ReservedNamespace
    {
        public ReservedNamespace(string nameSpace)
        {
            Namespace = nameSpace;
        }

        public string Namespace { get; }
        public List<ReservedFunction> ReservedFunctions { get; set; } = new List<ReservedFunction>();
    }
}
