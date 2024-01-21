using Newtonsoft.Json;

namespace AProtskiv.HashJson.Reserved
{
    public class ArgumentsToIgnore
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int[] AtJArrayIndecies { get; set; } = new int[] { };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] InJObjectProperties { get; set; } = new string[] { };
    }
}
