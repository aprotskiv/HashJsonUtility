using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AProtskiv.HashJson.Reserved
{
    public class NamespaceSettings : IReservedNamespaceSettings
    {
        [JsonIgnore]
        public Dictionary<string, ReservedNamespace> Items { get; set; } = new Dictionary<string, ReservedNamespace>();


        [JsonProperty]
        public List<ReservedNamespace> ReservedNamespaces
        {
            get
            {
                return Items.Values.ToList();
            }
        }

        [JsonProperty]
        public string Separator { get; } = ":";
    }
}