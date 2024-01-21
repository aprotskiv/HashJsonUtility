using System;

namespace AProtskiv.HashJson
{
    public class JsonAppException : Exception
    {
        public JsonAppException(string message) : base(message)
        {
        }
    }
}