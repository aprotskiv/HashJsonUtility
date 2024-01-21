using Newtonsoft.Json;
using System.Collections.Generic;

namespace AProtskiv.HashJson.Reserved
{

    public class ReservedNamespace
    {
        public ReservedNamespace(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Namespace name
        /// </summary>
        [JsonProperty]
        public string Name { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ReservedFunction> Functions { get; set; } = new List<ReservedFunction>();
    }

    public static class APP_ReservedNamespace_Consts
    {
        public const string HashChainOfSalt_PROPERTY = "app:hashChainOfSalt";
    }
}
