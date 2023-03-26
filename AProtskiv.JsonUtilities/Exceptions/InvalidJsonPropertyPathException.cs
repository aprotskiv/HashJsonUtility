namespace AProtskiv.JsonUtilities.Exceptions
{
    public class InvalidJsonPropertyPathException : InvalidJsonPathException
    {
        public InvalidJsonPropertyPathException(string path) : base(path)
        {
        }
    }
}
