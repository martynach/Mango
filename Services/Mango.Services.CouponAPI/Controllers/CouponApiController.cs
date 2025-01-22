using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponApi.Controllers;

[ApiController]
[Route("api/coupon")]
[Authorize]
public class CouponApiController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public CouponApiController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<ResponseDto<List<CouponDto>>> GetAllCoupons()
    {
        var response = new ResponseDto<List<CouponDto>>();

        try
        {
            var coupons = _dbContext.Coupons.ToList();
            response.Result = _mapper.Map<List<CouponDto>>(coupons);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }

    [HttpGet("{couponId:int}")]
    public ActionResult<ResponseDto<CouponDto>> GetCouponById([FromRoute] int couponId)
    {
        var response = new ResponseDto<CouponDto>();
        try
        {
            var coupon = _dbContext.Coupons.FirstOrDefault(coupon => coupon.CouponId == couponId);
            response.Result = _mapper.Map<CouponDto>(coupon);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    [HttpGet]
    [Route("GetByCode/{code}")]
    [AllowAnonymous] // todo to remove
    public ActionResult<ResponseDto<CouponDto>> GetCouponByCode([FromRoute] string code)
    {
        var response = new ResponseDto<CouponDto>();
        try
        {
            var coupon = _dbContext.Coupons.First(coupon => coupon.CouponCode.ToLower() == code.ToLower());
            response.Result = _mapper.Map<CouponDto>(coupon);
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public ActionResult<ResponseDto<CouponDto>> AddCoupon([FromBody] CouponDto dto)
    {
        var response = new ResponseDto<CouponDto>();
        try
        {
            var coupon = _mapper.Map<Coupon>(dto);
                _dbContext.Coupons.Add(coupon);
                _dbContext.SaveChanges();
                response.Result = dto;
                response.Message = $"Successfully added coupon id: {coupon.CouponId}";
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ActionResult<ResponseDto<CouponDto>> UpdateCoupon([FromBody] CouponDto dto)
    {
        var response = new ResponseDto<CouponDto>();
        try
        {
            
            
            var coupon = _mapper.Map<Coupon>(dto);
            var result = _dbContext.Coupons.Update(coupon);
            _dbContext.SaveChanges();
            response.Result = dto;
            response.Message = $"Successfully updated coupon with id: {result.Entity.CouponId}";
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
    
    
    [HttpDelete("{couponId:int}")]
    [Authorize(Roles = "ADMIN")]
    public ActionResult<ResponseDto<CouponDto>> DeleteCouponById([FromRoute] int couponId)
    {
        var response = new ResponseDto<CouponDto>();
        try
        {
            var coupon = _dbContext.Coupons.FirstOrDefault(coupon => coupon.CouponId == couponId);
            if (coupon is null)
            {
                response.Result = null;
                response.Message = $"Coupon with id: {couponId} not found";
            }
            else
            {
                var result = _dbContext.Coupons.Remove(coupon);
                _dbContext.SaveChanges();
                response.Result = _mapper.Map<CouponDto>(result.Entity);
            }

        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = e.Message;
        }

        return response;
    }
}