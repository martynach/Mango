using Mango.Services.ShoppingCartAPI.Models;

namespace Mango.Services.ShoppingCart.Services;

public interface ICouponService
{
    Task<Coupon?> GetCoupon(string couponCode);
}