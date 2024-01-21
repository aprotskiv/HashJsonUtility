namespace AProtskiv.JsonUtilities.NewtonsoftJson
{


    internal class NewtonsoftJson_Linq_JsonPath_ArrayIndexFilter : INewtonsoftJson_Linq_JsonPath_Filter, INewtonsoftJson_Linq_JsonPath_ArrayIndexFilter
    {
        private readonly object _objFilter;

        internal NewtonsoftJson_Linq_JsonPath_ArrayIndexFilter(object objFilter)
        {
            _objFilter = objFilter;
        }

        public int? Index
        {
            get
            {
                var index = NewtonsoftJson_Linq_JsonPath_JPath.ArrayIndexFilterType
                    .GetProperty("Index").GetValue(_objFilter) as int?;
                return index;
            }
        }
    }
}
