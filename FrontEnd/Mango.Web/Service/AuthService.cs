using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class AuthService: IAuthService
{
    private readonly IBaseService _baseService;

    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    
    public async Task<ResponseDto?> LoginAsync(LoginUserDto loginDto)
    {
        var result =  await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data=loginDto,
            Url = SD.AuthAPIBase + "/api/auth/login" 
        });

        return result;
    }
    
    public async Task<ResponseDto?> RegisterAsync(RegisterUserDto registerDto)
    {
        registerDto.ConfirmEmail = registerDto.Email;
        registerDto.ConfirmPassword = registerDto.Password;
        // todo 
        var result =  await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data=registerDto,
            Url = SD.AuthAPIBase + "/api/auth/register" 
        });

        return result;
    }
    public async Task<ResponseDto?> AssignRoleAsync(RegisterUserDto registerDto)
    {
        var result =  await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = registerDto,
            Url = SD.AuthAPIBase + "/api/auth/assignRole" 
        });

        return result;
    }
    
    
}