using Newtonsoft.Json;

namespace AProtskiv.HashJson.Reserved
{
    public class ReservedFunction
    {
        public ReservedFunction(
            //ReservedNamespace nameSpace, 
            string functionName, ArgumentsToIgnore args)
        {
            //this.nameSpace = nameSpace;
            FunctionName = functionName;
            Arguments = args;
        }

        //private readonly ReservedNamespace nameSpace;
        //public ReservedNamespace GetNamespace() => nameSpace;


        [JsonProperty]
        public string FunctionName { get; set; }

        [JsonProperty]
        public ArgumentsToIgnore Arguments { get; set; }
    }
}
