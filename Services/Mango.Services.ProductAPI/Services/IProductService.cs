using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Services;

public interface IProductService
{
    public Task<ResponseDto<List<ProductDto>>> GetProducts();
    public Task<ResponseDto<ProductDto>> GetProductById(int productId);
    public Task<ResponseDto<int>> CreateNewProduct(CreateProductDto dto);
    public Task<ResponseDto<int>> UpdateProduct(int productId, UpdateProductDto dto);
    public Task <ResponseDto<object>> DeleteById(int productId);




}