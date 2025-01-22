namespace Mango.Services.ShoppingCartAPI.Exceptions;

public class InvalidCartException : Exception
{
    public InvalidCartException(string message) : base(message)
    {
    }
}