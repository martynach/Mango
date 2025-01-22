using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[ApiController]
[Route("api/product")]
// [Authorize]
public class ProductApiController: ControllerBase
{
    private readonly IProductService _productService;

    public ProductApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetProducts();
        return Ok(products);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDto>> GetProductById([FromRoute] int productId)
    {
        var product = await _productService.GetProductById(productId);
        return Ok(product);
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        var responseDto = await _productService.CreateNewProduct(dto);
        return Created($"api/product/{responseDto.Result}", responseDto);
    }
    
    [HttpPut("{productId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> UpdateProduct([FromRoute] int productId, [FromBody] UpdateProductDto dto)
    {
        var responseDto = await _productService.UpdateProduct(productId, dto);
        return Ok(responseDto);
    }
    
    [HttpDelete("{productId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult> DeleteById([FromRoute] int productId)
    {
        var responseDto = await _productService.DeleteById(productId);
        return Ok(responseDto);
    }
}