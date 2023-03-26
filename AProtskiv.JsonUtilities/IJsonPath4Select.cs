using System.Collections.Generic;

namespace AProtskiv.JsonUtilities
{
    public interface IJsonPath4Select
    {
        /// <summary>
        /// Returns TRUE if path is not valid "Newtonsoft.Json.Linq.JsonPath.JPath"
        /// </summary>
        bool IsNotValidNewtonsoftJsonPath { get; }

        /// <summary>
        /// List of segments of valid Json path.
        /// May contains string (Json object property) or integer (Json array index) elements.
        /// </summary>
        IReadOnlyList<object> Segments { get; }
    }
}