using System;
using System.Collections.Generic;
using System.Linq;

namespace AProtskiv.JsonUtilities
{

	/// <summary>
	/// Use to SELECT data from DB only.<para/>
	/// Does NOT throw when 'newtonsoftJsonPath' is not valid
	/// </summary>
	public sealed class JsonPath4Select : JsonPath, IJsonPath4Select
	{
		/// <summary>
		/// Does NOT throw when <paramref name="newtonsoftJsonPath"/> is not valid
		/// </summary>
		public JsonPath4Select(string newtonsoftJsonPath)
		{
			try
			{
				base.Segments = ParseSegments(newtonsoftJsonPath);
			}
			catch
			{
				IsNotValidNewtonsoftJsonPath = true;
			}
		}

		/// <summary>
		/// Constructor for NON-valid path
		/// </summary>
		private JsonPath4Select()
		{
			IsNotValidNewtonsoftJsonPath = true;
		}

		public JsonPath4Select(IList<object> segments)
		{
			base.Segments = segments?.ToList() ?? new List<object>();
		}

		public bool IsNotValidNewtonsoftJsonPath { get; }

		public new IReadOnlyList<object> Segments => base.Segments;

		/// <summary>
		/// Does NOT throw when <paramref name="postgressJsonPath"/> is not valid
		/// </summary>
		public static JsonPath4Select FromPostgresJsonPath(string postgressJsonPath)
		{
			try
			{
				if (postgressJsonPath.StartsWith("{") && postgressJsonPath.EndsWith("}"))
				{
					var postgressSegmentsRoot = postgressJsonPath.Substring(1, postgressJsonPath.Length - 2);
					var postgressSegments = postgressSegmentsRoot.Split(new char[] { ',' });

					var validSegments = new List<object>();

					string unclosedPreviousSegment = null;
					foreach (var segm in postgressSegments)
					{
						object validSegment;

						if (segm.EndsWith("\""))
						{
							if (unclosedPreviousSegment != null)
							{
								validSegment = unclosedPreviousSegment;
								unclosedPreviousSegment = null;
							}
							else if (segm.StartsWith("\""))
							{
								validSegment = segm;
							}
							else
							{
								throw new Exception($"{segm} is not valid segment");
							}
						}
						else if (unclosedPreviousSegment != null)
						{
							unclosedPreviousSegment += "," + segm;
							validSegment = null;
						}
						else if (segm.StartsWith("\""))
						{
							unclosedPreviousSegment = segm;
							validSegment = null;
						}
						else if (Int32.TryParse(segm, out int segmAsUInt32))
						{
							validSegment = segmAsUInt32;
						}
						else
						{
							validSegment = segm.Trim(); // trim " b1" in Postgres JsonPath "{a1, b1}"
						}

						if (validSegment != null)
						{
							validSegments.Add(validSegment);
						}
					}

					if (unclosedPreviousSegment != null)
					{
						throw new Exception($"{unclosedPreviousSegment} is not valid segment");
					}

					var newtonsoftJsonPath = NewtonsoftJsonPathBuilder.Build(validSegments);
					return new JsonPath4Select(newtonsoftJsonPath);
				}
			}
			catch
			{
			}

			// return invalid
			return new JsonPath4Select();
		}

		/// <summary>
		/// Does NOT throw when <paramref name="newtonsoftJsonPath"/> is not valid
		/// </summary>
		public static JsonPath4Select FromWellFormedNewtonsoftJsonPath(string newtonsoftJsonPath)
		{
			try
			{
				if (newtonsoftJsonPath.StartsWith("$"))
				{
					return new JsonPath4Select(newtonsoftJsonPath);
				}
			}
			catch
			{
			}

			// return invalid
			return new JsonPath4Select();
		}
	}
}