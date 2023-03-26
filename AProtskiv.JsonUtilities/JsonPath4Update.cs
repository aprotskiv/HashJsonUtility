using System;
using System.Collections.Generic;
using System.Linq;

namespace AProtskiv.JsonUtilities
{
	/// <summary>
	/// for update actions: Insert/Set/Delete/Rename/Duplicate etc.
	/// </summary>
	public sealed class JsonPath4Update : JsonPath
	{
		const string ArrayIndexPattern = "[*]";
		private JsonPath4Update()
		{
		}

		public JsonPath4Update(JsonPath source)
		{
			this.Segments = source.Segments.ToList();
		}

		public JsonPath4Update(string newtonsoftJsonPath)
		{
			try
			{
				this.Segments = ParseSegmentsWithArrayBrackets(newtonsoftJsonPath);
			}
			catch (Exception exc)
			{
				throw new ArgumentException($"'{newtonsoftJsonPath}' is not valid NewtonsoftJson Path pattern for Update.", paramName: nameof(newtonsoftJsonPath), exc);
			}
		}

		public static List<object> ParseSegmentsWithArrayBrackets(string s)
		{
			var result = new List<object>();

			var ppIndex = s.IndexOf(ArrayIndexPattern); // check if contains '[]' 
			if (ppIndex >= 0)
			{
				if (ppIndex > 0)
				{
					result.AddRange(
						ParseSegments(s.Substring(0, ppIndex))
					);
				}
				result.Add(ArrayIndexPattern);
				result.AddRange(ParseSegmentsWithArrayBrackets(s.Substring(ppIndex + ArrayIndexPattern.Length)));
			}
			else
			{
				result.AddRange(ParseSegments(s));
			}

			return result;
		}

		/// <summary>
		/// Check whether one of segments is '[]'
		/// </summary>
		public bool HasArrayIndexPattern()
		{
			return this.Segments.Contains(ArrayIndexPattern);
		}

		public bool IsLastSegmentArrayIndexPattern()
		{
			var segment = Segments.LastOrDefault();
			return segment?.GetType() == typeof(string) && ArrayIndexPattern.Equals(segment); //  is '[]'
		}

		public bool IsLastSegmentProperty()
		{
			var segment = Segments.LastOrDefault();
			return segment?.GetType() == typeof(string) // is String
				&& !segment.Equals(ArrayIndexPattern); //  is NOT '[]'
		}
	}
}
