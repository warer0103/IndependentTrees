namespace IndependentTrees.API.Exceptions
{
    public class SecureException : Exception
    {
        public readonly string Type = "Secure";

        public SecureException(string message) : base(message) 
        { }
    }
}
