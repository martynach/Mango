using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.ApplyCoupon;
using Mango.Services.ShoppingCartAPI.Models.Dto.Upsert;
using Mango.Services.ShoppingCartAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class ShoppingCartApiController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IMessageBus _messageBus;
    private readonly IConfiguration _configuration;


    public ShoppingCartApiController(IShoppingCartService shoppingCartService, IMessageBus messageBus, IConfiguration configuration)
    {
        _shoppingCartService = shoppingCartService;
        _messageBus = messageBus;
        _configuration = configuration;
    }

    [HttpPost("CartUpsert")]
    public async Task<IActionResult> CartUpsert(UpsertCartDto cartDto)
    {
        await _shoppingCartService.CartUpsert(cartDto);
        return Ok("Shopping cart successfully created/updated");
    }
    
    [HttpPost("CartDelete")] 
    public async Task<IActionResult> DeleteCartDetails([FromBody] int cartDetailsId)
    {
        var message = await _shoppingCartService.DeleteCartDetails(cartDetailsId);
        return Ok(message);
    }

    [HttpGet("CartGet/{userId}")]
    public async Task<IActionResult> GetShoppingCart([FromRoute] string userId)
    {
        var dto = await _shoppingCartService.GetShoppingCart(userId);
        return Ok(dto);
    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] ApplyCouponDto applyCouponDto)
    {
        await _shoppingCartService.ApplyCoupon(applyCouponDto);
        return Ok();
    }
    
    [HttpPost("EmailCartRequest")]
    public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
    {
        var queueName = _configuration.GetValue<string>("QueueAndTopicNames:ShoppingCartEmail");
        await _messageBus.PublishMessage(cartDto, queueName);
        return Ok();
    }
    
}