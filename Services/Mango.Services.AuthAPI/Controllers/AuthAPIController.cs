using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models.Dtos;
using Mango.Services.AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthApiController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
    {
        var response = await _userService.RegisterUser(dto);
        // if (!response.IsSuccess)
        // {
        //     return BadRequest(response);
        // }
        return Ok(response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto dto)
    {
        var loginResponseDto = await _userService.LoginUser(dto);
        if (loginResponseDto.User is null)
        {
            var errorResponse = new ResponseDto<LoginResponseDto>()
            {
                IsSuccess = false,
                Message = "Login failed"
            };
            return BadRequest(errorResponse);
        }

        var response = new ResponseDto<LoginResponseDto>()
        {
            Result = loginResponseDto
        };
        return Ok(response);
    }
    
    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRole([FromBody] RegisterUserDto dto)
    {
        var isAssigned = await _userService.AssignRole(dto.Email, dto.RoleName.ToUpper());
        if (!isAssigned)
        {
            var errorResponse = new ResponseDto<bool>()
            {
                IsSuccess = false,
                Message = "Assigning role failed"
            };
            return BadRequest(errorResponse);
        }

        var response = new ResponseDto<bool>()
        {
            Result = true
        };
        return Ok(response);
    }

}