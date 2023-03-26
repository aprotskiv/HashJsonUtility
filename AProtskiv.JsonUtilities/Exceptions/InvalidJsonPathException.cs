using System;

namespace AProtskiv.JsonUtilities.Exceptions
{
    public class InvalidJsonPathException : ArgumentOutOfRangeException
    {
        public InvalidJsonPathException(string path) : base(path)
        {
        }
    }
}
