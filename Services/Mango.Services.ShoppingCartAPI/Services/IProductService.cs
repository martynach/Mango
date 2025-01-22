using Mango.Services.ShoppingCartAPI.Models;

namespace Mango.Services.ShoppingCart.Services;

public interface IProductService
{
    Task<Product> GetProduct(int productId);
}