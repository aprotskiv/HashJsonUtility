using System;

namespace AProtskiv.JsonUtilities.NewtonsoftJson
{
    public interface INewtonsoftJson_Linq_JsonPath_Filter
    {
    }

    public interface INewtonsoftJson_Linq_JsonPath_FieldFilter : INewtonsoftJson_Linq_JsonPath_Filter
    {
        string Name { get; }
    }

    public interface INewtonsoftJson_Linq_JsonPath_ArrayIndexFilter
    {
        int? Index { get; }
    }
}
