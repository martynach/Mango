﻿using Mango.Services.ProductAPI.Models.Dto;
using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IProductService
{
    Task<ResponseDto?> GetAllProductsAsync();
    Task<ResponseDto?> GetProductByIdAsync(int id);
    Task<ResponseDto?> CreateProductAsync(CreateProductDto productDto);
    Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
    Task<ResponseDto?> DeleteProductAsync(int id);
}