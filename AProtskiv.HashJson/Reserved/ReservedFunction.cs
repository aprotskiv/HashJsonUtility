namespace AProtskiv.HashJson.Reserved
{
    public class ReservedFunction
    {
        public ReservedFunction(ReservedNamespace nameSpace, string function, ArgumentsToIgnore args)
        {
            Namespace = nameSpace;
            Function = function;
            Arguments = args;
        }

        public ReservedNamespace Namespace { get; set; }
        public string Function { get; set; }

        public ArgumentsToIgnore Arguments { get; set; }

        public class ArgumentsToIgnore
        {
            public int[] AtJArrayIndecies { get; set; } = new int[] { };
            public string[] InJObjectProperties { get; set; } = new string[] { };
        }
    }
}
