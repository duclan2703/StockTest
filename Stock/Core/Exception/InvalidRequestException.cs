namespace Core.Exception
{
    public class InvalidRequestException : System.Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }
}
