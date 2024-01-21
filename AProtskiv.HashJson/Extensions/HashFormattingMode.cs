using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AProtskiv.HashJson.Extensions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HashFormattingMode
    {
        [JsonProperty]
        Base64 = 0,

        [JsonProperty]
        Hexadecimal = 1,
    }
}
