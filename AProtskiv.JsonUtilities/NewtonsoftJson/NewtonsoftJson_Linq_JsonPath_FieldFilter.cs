namespace AProtskiv.JsonUtilities.NewtonsoftJson
{
    internal class NewtonsoftJson_Linq_JsonPath_FieldFilter : INewtonsoftJson_Linq_JsonPath_Filter, INewtonsoftJson_Linq_JsonPath_FieldFilter
    {
        private readonly object _objFilter;

        internal NewtonsoftJson_Linq_JsonPath_FieldFilter(object objFilter)
        {
            _objFilter = objFilter;
        }

        public string Name
        {
            get
            {
                //internal field
                var fName = NewtonsoftJson_Linq_JsonPath_JPath.FieldFilterType
                    .GetField("Name", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var name = fName.GetValue(_objFilter) as string;
                return name;
            }
        }
    }
}
