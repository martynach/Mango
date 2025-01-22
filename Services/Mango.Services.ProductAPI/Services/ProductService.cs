using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Exceptions;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Services;

public class ProductService: IProductService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<ResponseDto<List<ProductDto>>> GetProducts()
    {
        var products = await _dbContext.Products.ToListAsync();
        var dtos = _mapper.Map<List<ProductDto>>(products);
        var responseDto = new ResponseDto<List<ProductDto>>()
        {
            Result = dtos,
        };
        return responseDto;
    }

    public async Task<ResponseDto<ProductDto>> GetProductById(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product is null)
        {
            throw new NotFoundException($"Product with id: {productId} does not exist");
        }
        var dto = _mapper.Map<ProductDto>(product);
        var responseDto = new ResponseDto<ProductDto>()
        {
            Result = dto
        };
        return responseDto;
    }
    
    public async Task<ResponseDto<int>> CreateNewProduct(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        
        var result = await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        var responseDto = new ResponseDto<int>()
        {
            Result = result.Entity.ProductId
        };

        return responseDto;
    }
    
    public async Task<ResponseDto<int>> UpdateProduct(int productId, UpdateProductDto dto)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product is null)
        {
            throw new NotFoundException($"Product with id: {productId} does not exist");
        }

        product.CategoryName = dto.CategoryName ?? product.CategoryName;
        product.Name = dto.Name ?? product.Name;
        product.Description = dto.Description ?? product.Description;
        product.Price = dto.Price ?? product.Price;
        product.ImageUrl = dto.ImageUrl ?? product.ImageUrl;

        var result = _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();

        return new ResponseDto<int>() { Result = result.Entity.ProductId };
    }

    public async Task<ResponseDto<object>> DeleteById(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product is null)
        {
            throw new NotFoundException($"Product with id: {productId} does not exist");
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
        return new ResponseDto<object>();
    }
    
}