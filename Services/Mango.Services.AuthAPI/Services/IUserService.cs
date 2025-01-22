using Mango.Services.AuthAPI.Models.Dtos;

namespace Mango.Services.AuthAPI.Services;

public interface IUserService
{
    // public Task<ResponseDto<Object>> RegisterUser(RegisterUserDto dto);
    // public Task<string> LoginUser(LoginUserDto dto);
    
    public Task<ResponseDto<Object>> RegisterUser(RegisterUserDto dto);
    public Task<LoginResponseDto> LoginUser(LoginUserDto dto);

    public Task<bool> AssignRole(string email, string roleName);
}