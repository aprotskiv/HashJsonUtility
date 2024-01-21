using System;
using System.Collections.Generic;
using System.Reflection;

namespace AProtskiv.JsonUtilities.NewtonsoftJson
{
    public class NewtonsoftJson_Linq_JsonPath_JPath
    {
        internal static Assembly Assembly => typeof(Newtonsoft.Json.JsonConvert).Assembly;

        internal static Type FieldFilterType => Assembly.GetType("Newtonsoft.Json.Linq.JsonPath.FieldFilter");
        internal static Type ArrayIndexFilterType => Assembly.GetType("Newtonsoft.Json.Linq.JsonPath.ArrayIndexFilter");

        private readonly List<INewtonsoftJson_Linq_JsonPath_Filter> _filters = new List<INewtonsoftJson_Linq_JsonPath_Filter>();

        public NewtonsoftJson_Linq_JsonPath_JPath(string newtonsoftJsonPath)
        {
            var normalizedInput = newtonsoftJsonPath
                .TrimStart(new[] { '$', '.' })
                .TrimEnd(new[] { '.' });

            var tJsonPath = Assembly.GetType("Newtonsoft.Json.Linq.JsonPath.JPath");
            var objJsonPath = Activator.CreateInstance(tJsonPath, new object[] { normalizedInput });

            var filters = tJsonPath.GetProperty("Filters").GetValue(objJsonPath) as System.Collections.IEnumerable;
            var enumerator = filters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var objFilter = enumerator.Current;
                if (objFilter.GetType() == FieldFilterType)
                {
                    _filters.Add(
                        new NewtonsoftJson_Linq_JsonPath_FieldFilter(objFilter)
                    );
                }
                else if (objFilter.GetType() == ArrayIndexFilterType)
                {
                    _filters.Add(
                        new NewtonsoftJson_Linq_JsonPath_ArrayIndexFilter(objFilter)
                    );
                }
            }
        }

        public IReadOnlyCollection<INewtonsoftJson_Linq_JsonPath_Filter> Filters => _filters;

    }
}