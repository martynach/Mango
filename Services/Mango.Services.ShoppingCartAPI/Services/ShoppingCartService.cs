using AutoMapper;
using Mango.Services.ShoppingCart.Services;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Exceptions;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.ApplyCoupon;
using Mango.Services.ShoppingCartAPI.Models.Dto.Upsert;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.ShoppingCartAPI.Services;

public class ShoppingCartService: IShoppingCartService
{
    private readonly AppDbContext _appDbContext;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;

    public ShoppingCartService (
        AppDbContext appDbContext,
        IMapper mapper,
        IProductService productService,
        ICouponService couponService)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
        _productService = productService;
        _couponService = couponService;
    }
    
    public async Task CartUpsert(UpsertCartDto cartDto)
    {
        var existingCartHeader =
            _appDbContext.CartHeaders.FirstOrDefault(ch => ch.UserId == cartDto.CartHeader.UserId);

        if (existingCartHeader is null)
        {
            await CreateNewShoppingCart(cartDto);
        }
        else
        {
            await UpdateExistingShoppingCart(cartDto, existingCartHeader.CartHeaderId);
        }    
    }

    public async Task<string> DeleteCartDetails(int cartDetailsId)
    {
        var cartDetails = await _appDbContext.CartDetails
            .FirstOrDefaultAsync(cd => cd.CartDetailsId == cartDetailsId);
        
        if (cartDetails is null)
        {
            throw new NotFoundException($"Cart Details with id: {cartDetailsId} not found");
        }

        var cartHeaderId = cartDetails.CartHeaderId;
        var count = await _appDbContext.CartDetails
            .Where(cd => cd.CartHeaderId == cartHeaderId)
            .CountAsync();

        var additionalResponseMessage = "";
        if (count == 1)
        {
            var cartHeader = await _appDbContext.CartHeaders.FirstAsync(ch => ch.CartHeaderId == cartHeaderId);
            _appDbContext.CartHeaders.Remove(cartHeader);
            additionalResponseMessage = " The whole shopping cart is removed";
        }
        _appDbContext.CartDetails.Remove(cartDetails);
        await _appDbContext.SaveChangesAsync();

        return "Shopping cart details successfully deleted." + additionalResponseMessage;
    }

    public async Task<CartDto> GetShoppingCart(string userId)
    {
        var shoppingCartHeader = await _appDbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);
        if (shoppingCartHeader is null)
        {
            throw new NotFoundException($"Shopping cart for the user with id: {userId} not found");
        }

        var cartDetails = await _appDbContext.CartDetails
            .Where(cd => cd.CartHeaderId == shoppingCartHeader.CartHeaderId)
            .ToListAsync();

        var dto = new CartDto()
        {
            CartHeader = _mapper.Map<CartHeaderDto>(shoppingCartHeader),
            CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(cartDetails)
        };
        
        await CalculateCartTotal(dto);
        await CalculateCouponDiscount(dto);
        return dto;
    }

    private async Task CalculateCouponDiscount(CartDto dto)
    {
        if (String.IsNullOrWhiteSpace(dto.CartHeader.CouponCode))
        {
            return;
        }
        
        var coupon = await _couponService.GetCoupon(dto.CartHeader.CouponCode);
        if (coupon is not null && coupon.MinAmount <= dto.CartHeader.CartTotal)
        {
            dto.CartHeader.CartTotal -= coupon.DiscountAmount;
            dto.CartHeader.Discount = coupon.DiscountAmount;
        }
    }


    private async Task CalculateCartTotal(CartDto dto)
    {
        foreach (var item in dto.CartDetails)
        {
            var product = await _productService.GetProduct(item.ProductId);
            item.Product = _mapper.Map<ProductDto>(product);
            var currentProductsPrice = item.Count * item.Product.Price;
            dto.CartHeader.CartTotal += currentProductsPrice;
        }
    }

    public async Task ApplyCoupon(ApplyCouponDto applyCouponDto)
    {
        var cartFromDb = await _appDbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == applyCouponDto.UserId);
        if (cartFromDb is null)
        {
            throw new NotFoundException($"Cart for user with id: {applyCouponDto.UserId} is not found");
        }
        cartFromDb.CouponCode = applyCouponDto.CouponCode;
        _appDbContext.CartHeaders.Update(cartFromDb);
        await _appDbContext.SaveChangesAsync();
    }
    
    private async Task CreateNewShoppingCart(UpsertCartDto cartDto)
    {
        var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
        if (cartDto.CartDetails.IsNullOrEmpty())
        {
            throw new InvalidCartException("When creating new shopping cart for the user cart details are required.");
        }

        var cartHeaderEntry = await _appDbContext.CartHeaders.AddAsync(cartHeader);
        await _appDbContext.SaveChangesAsync();

        var cartHeaderId = cartHeaderEntry.Entity.CartHeaderId;
        var cartDetails = _mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails);
        foreach (var cd in cartDetails)
        {
            cd.CartHeaderId = cartHeaderId;
        }

        await _appDbContext.CartDetails.AddRangeAsync(cartDetails);

        await _appDbContext.SaveChangesAsync();
    }
    
    private async Task UpdateExistingShoppingCart(UpsertCartDto cartDto, int cartHeaderId)
    {
        foreach (var dto in cartDto.CartDetails)
        {
            var cartDetails = await _appDbContext.CartDetails
                .FirstOrDefaultAsync(cd => cd.ProductId == dto.ProductId && cd.CartHeaderId == cartHeaderId);
            if (cartDetails is null)
            {
                await CreateNewCartDetails(cartHeaderId, dto);
            }
            else
            {
                cartDetails.Count += dto.Count;
                _appDbContext.CartDetails.Update(cartDetails);
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
    
    private async Task CreateNewCartDetails(int cartHeaderId, UpsertCartDetailsDto dto)
    {
        var cartDetails = _mapper.Map<CartDetails>(dto);
        cartDetails.CartHeaderId = cartHeaderId;
        await _appDbContext.CartDetails.AddAsync(cartDetails);
        await _appDbContext.SaveChangesAsync();
    }
}