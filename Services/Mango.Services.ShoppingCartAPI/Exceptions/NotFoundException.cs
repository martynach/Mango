namespace Mango.Services.ShoppingCartAPI.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}