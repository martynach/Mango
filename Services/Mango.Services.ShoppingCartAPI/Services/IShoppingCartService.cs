using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.ApplyCoupon;
using Mango.Services.ShoppingCartAPI.Models.Dto.Upsert;

namespace Mango.Services.ShoppingCartAPI.Services;

public interface IShoppingCartService
{
    public Task CartUpsert(UpsertCartDto cartDto);
    public Task<string> DeleteCartDetails(int cartDetailsId);
    public Task<CartDto> GetShoppingCart(string userId);
    public Task ApplyCoupon(ApplyCouponDto applyCouponDto);
}