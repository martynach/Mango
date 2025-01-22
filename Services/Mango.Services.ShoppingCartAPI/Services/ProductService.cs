using System.Net;
using Mango.Services.ShoppingCartAPI.Exceptions;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCart.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Product> GetProduct(int productId)
    {
        var httpClient = _httpClientFactory.CreateClient("product");
        HttpResponseMessage? apiResponse = await httpClient.GetAsync($"/api/product/{productId}");

        if (apiResponse.IsSuccessStatusCode)
        {
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            var product = JsonConvert.DeserializeObject<Product>(Convert.ToString(apiResponseDto.Result));
            return product;
        }

        switch (apiResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new InvalidCartException($"Product with id: {productId} not found");
            case HttpStatusCode.Forbidden:
                throw new Exception($"Access Denied");
            case HttpStatusCode.Unauthorized:
                throw new Exception($"Unauthorized");
            case HttpStatusCode.BadRequest:
                throw new InvalidCartException($"Got bad request");
            default:
                throw new Exception("Some Unknown failure");
        }
    }
}