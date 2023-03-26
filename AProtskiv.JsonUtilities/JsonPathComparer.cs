using System.Collections.Generic;

namespace AProtskiv.JsonUtilities
{
    public class JsonPathComparer : IComparer<JsonPath>
	{
		public int Compare(JsonPath x, JsonPath y)
		{
			return x.CompareTo(y);
		}
	}
}
