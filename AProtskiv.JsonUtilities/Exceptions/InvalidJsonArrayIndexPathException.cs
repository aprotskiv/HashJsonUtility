namespace AProtskiv.JsonUtilities.Exceptions
{
    public class InvalidJsonArrayIndexPathException : InvalidJsonPathException
    {
        public InvalidJsonArrayIndexPathException(string path) : base(path)
        {
        }
    }
}
