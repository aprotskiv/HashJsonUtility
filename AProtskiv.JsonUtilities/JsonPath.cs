using AProtskiv.JsonUtilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AProtskiv.JsonUtilities
{

    public class JsonPath : IComparable<JsonPath>
	{
		/// <summary>
		/// Path for root token
		/// </summary>
		public static JsonPath Root
		{
			get
			{
				return new JsonPath();
			}
		}

		public static JsonPath FromProperty(string property)
		{
			return new JsonPath()
			{
				Segments = new object[] { property }
			};
		}

		protected JsonPath()
		{
		}

		/// <summary>
		/// Properly parses NewtonsoftJson Path segments <para/>
		/// For example .['Browser History'][49] <para/>
		/// String.Empty and '.' are ignored
		/// </summary>
		/// <param name="newtonsoftJsonPath"></param>
		public static List<object> ParseSegments(string newtonsoftJsonPath)
		{
			var normalizedInput = newtonsoftJsonPath
					.TrimStart(new[] { '$', '.' })
					.TrimEnd(new[] { '.' });

			List<object> segmentsWithZeroBasedIndicies = new List<object>();

			if (normalizedInput.Length > 0)
			{
				var assembly = typeof(Newtonsoft.Json.JsonConvert).Assembly;

				var tJsonPath = assembly.GetType("Newtonsoft.Json.Linq.JsonPath.JPath");
				var tArrayIndexFilter = assembly.GetType("Newtonsoft.Json.Linq.JsonPath.ArrayIndexFilter");
				var tFieldFilter = assembly.GetType("Newtonsoft.Json.Linq.JsonPath.FieldFilter");

				var objJsonPath = Activator.CreateInstance(tJsonPath, new object[] { normalizedInput });
				var filters = tJsonPath.GetProperty("Filters").GetValue(objJsonPath) as System.Collections.IEnumerable;

				var enumerator = filters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var objFilter = enumerator.Current;
					if (objFilter.GetType() == tArrayIndexFilter)
					{
						//public property
						var index = tArrayIndexFilter.GetProperty("Index").GetValue(objFilter) as int?;
						if (index.HasValue)
						{
							segmentsWithZeroBasedIndicies.Add(index.Value);
						}
						else
						{
							// ??
						}
					}
					else if (objFilter.GetType() == tFieldFilter)
					{
						//internal field
						var fName = tFieldFilter.GetField("Name", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
						var name = fName.GetValue(objFilter) as string;

						if (!string.IsNullOrEmpty(name))
						{
							segmentsWithZeroBasedIndicies.Add(name);
						}
						else
						{
						}
					}
				}
			}
			return segmentsWithZeroBasedIndicies;
		}

		protected internal IReadOnlyList<object> Segments { get; set; } = new List<object>();

		public string ToNewtonsoftJsonPath()
		{
			return NewtonsoftJsonPathBuilder.Build(this.Segments);
		}

		/// <summary>
		/// '{"path1","path2",3}'::text[]
		/// </summary>
		/// <returns></returns>
		public string ToPostgresTextArray()
		{
			return "'{" + this.ToString() + "}'::text[] ";
		}

		private static string EscapePostgresProperty(string property)
		{
			return $"\"{property}\"";
		}

		/// <summary>
		/// Escaped (for Postgres)
		/// </summary>
		public override string ToString()
		{
			if (Segments == null)
				return String.Empty;

			return String.Join(",", Segments.Select(x =>
				x is string property
				? EscapePostgresProperty(property)
				: x.ToString()
			));
		}

		public bool IsEmpty()
		{
			return Segments == null;
		}

		public bool IsRoot()
		{
			return Segments != null && !Segments.Any();
		}

		/// <summary>
		/// Create new path without last segment
		/// </summary>
		/// <returns></returns>
		public JsonPath TrimLastSegmentAsNew()
		{
			var copy = new JsonPath()
			{
				Segments = this.Segments
			};
			copy.TrimLastSegment();
			return copy;
		}

		/// <summary>
		/// Removes last segment
		/// </summary>
		public void TrimLastSegment()
		{
			if (this.Segments.Any())
			{
				Segments = this.Segments.Take(this.Segments.Count - 1).ToList();
			}
		}

		/// <summary>
		/// Creates new path
		/// </summary>
		/// <param name="lastSegment"></param>
		/// <returns></returns>
		public JsonPath AppendLastSegment(object lastSegment)
		{
			return new JsonPath
			{
				Segments = this.Segments.Concat(new[] { lastSegment })
					.ToList()
			};
		}

		public string LastSegmentAsProperty()
		{
			return Convert.ToString(Segments.Last());
		}

		public bool IsLastSegmentArrayIndex()
		{
			return Segments.LastOrDefault()?.GetType() == typeof(int);
		}

		public int LastSegmentAsArrayIndex()
		{
			return Convert.ToInt32(Segments.Last());
		}

		public override int GetHashCode()
		{
			int hc = 0;
			if (Segments != null)
				foreach (var p in Segments)
					hc ^= p.GetHashCode();
			return hc;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is JsonPath objJsonPath))
				return base.Equals(obj);

			return this.CompareTo(objJsonPath) == 0;
		}

		public int CompareTo(JsonPath other)
		{
			int index = 0;

			while (true)
			{
				var thisSegment = this.Segments.ElementAtOrDefault(index);
				var otherSegment = other.Segments.ElementAtOrDefault(index);

				var segmentComparisonResult = CompareSegments(thisSegment, otherSegment);

				if (segmentComparisonResult != 0 || thisSegment == null || otherSegment == null)
					return segmentComparisonResult;

				index++;
			}
		}


		/// <summary>
		/// Determines whether Path points to last array element ()
		/// </summary>
		public bool InsertIntoArrayEnd()
		{
			return (-1).Equals(Segments.LastOrDefault());
		}

		private static int CompareSegments(object a, object b)
		{
			if (a == null && b == null)
				return 0;

			if (a == null && b != null)
				return -1;

			if (a != null && b == null)
				return 1;

			if (a is string && b is string)
				return StringComparer.InvariantCulture.Compare(a, b);

			if (a is string && !(b is string))
				return 1;

			if (!(a is string) && b is string)
				return -1;

			if (int.TryParse(a.ToString(), out int aNumber) && int.TryParse(b.ToString(), out int bNumber))
				return aNumber.CompareTo(bNumber);

			throw new ArgumentException($"Unable to compare two segments: 1) '{a}' ({a.GetType().FullName}) and 2) '{b}' ({b.GetType().FullName})");
		}

		public JsonPath ChangeLastSegment(int lastSegment)
		{
			return ChangeSegment(lastSegment, lastSegmentMustBeProperty: false, lastSegmentMustBeIndex: true);
		}

		public JsonPath ChangeLastSegment(string lastSegment)
		{
			bool lastSegmentMustBeProperty;
			bool lastSegmentMustBeIndex;

			object objLastSegment;

			if (int.TryParse(lastSegment, out int lastSegmentAsInt))
			{
				lastSegmentMustBeIndex = true;
				lastSegmentMustBeProperty = false;
				objLastSegment = lastSegmentAsInt;
			}
			else if (String.IsNullOrEmpty(lastSegment))
			{
				lastSegmentMustBeIndex = true;
				lastSegmentMustBeProperty = false;
				objLastSegment = -1;
			}
			else
			{
				lastSegmentMustBeIndex = false;
				lastSegmentMustBeProperty = true;
				objLastSegment = lastSegment;
			}

			return ChangeSegment(objLastSegment, lastSegmentMustBeProperty: lastSegmentMustBeProperty, lastSegmentMustBeIndex: lastSegmentMustBeIndex);
		}

		/// <summary>
		/// Create a copy with new last key
		/// </summary>
		public JsonPath ChangeLastKey(string newPropertyName)
		{
			return ChangeSegment(newPropertyName, lastSegmentMustBeProperty: true, lastSegmentMustBeIndex: false);
		}

		/// <summary>
		/// Create a copy with new last key
		/// </summary>
		/// <param name="newLastSegment"></param>
		/// <returns></returns>
		private JsonPath ChangeSegment(object newLastSegment, bool lastSegmentMustBeProperty, bool lastSegmentMustBeIndex)
		{
			var lastSegment = this.Segments.LastOrDefault();
			if (lastSegment == null)
				throw new Exception("JsonPath is empty");

			if (!(lastSegment is String))
			{
				if (lastSegmentMustBeProperty)
					throw new InvalidJsonPropertyPathException($"JsonPath last segment '{lastSegment}' is not a valid JSON key");
			}
			if (!(lastSegment is int))
			{
				if (lastSegmentMustBeIndex)
					throw new InvalidJsonArrayIndexPathException($"JsonPath last segment '{lastSegment}' is not a valid JSON array index");
			}

			var newSegments = this.Segments.Take(this.Segments.Count - 1).ToList();
			newSegments.Add(newLastSegment);

			return new JsonPath()
			{
				Segments = newSegments
			};
		}
	}
}
