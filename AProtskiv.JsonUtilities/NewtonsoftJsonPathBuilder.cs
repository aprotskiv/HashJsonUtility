using System.Collections.Generic;
using System.Text;

namespace AProtskiv.JsonUtilities
{
	public class NewtonsoftJsonPathBuilder
	{
		public static string Build(IReadOnlyList<object> segments)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < segments.Count; i++)
			{
				var segment = segments[i];
				if (segment is int)
				{
					sb.Append('[').Append(segment).Append(']');
				}
				else
				{
					if (i > 0)
						sb.Append('.');

					sb.Append("[\'").Append(segment).Append("\']"); // escape property (with spaces)					
				}
			}
			return sb.ToString();
		}
	}
}
