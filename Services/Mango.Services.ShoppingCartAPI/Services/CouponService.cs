using System.Net;
using Mango.Services.ShoppingCartAPI.Exceptions;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCart.Services;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Coupon> GetCoupon(string couponCode)
    {
        var httpClient = _httpClientFactory.CreateClient("coupon");
        HttpResponseMessage? apiResponse = await httpClient.GetAsync($"/api/coupon/GetByCode/{couponCode}");

        if (apiResponse.IsSuccessStatusCode)
        {
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            var coupon = JsonConvert.DeserializeObject<Coupon>(Convert.ToString(apiResponseDto.Result));
            return coupon;
        }

        switch (apiResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new InvalidCartException($"Coupon with code: {couponCode} not found");
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