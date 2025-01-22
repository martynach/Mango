using dot_net_api;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGeneratorService _tokenGeneratorService;
    private readonly RoleManager<IdentityRole> _roleManager;


    public UserService(
        AppDbContext dbContext,
        ILogger<UserService> logger,
        UserManager<ApplicationUser> userManager,
        IJwtTokenGeneratorService tokenGeneratorService,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
        _tokenGeneratorService = tokenGeneratorService;
        _roleManager = roleManager;
    }
    
    public async Task<ResponseDto<Object>> RegisterUser(RegisterUserDto dto)
    {
        _logger.LogInformation("Register user - start");
        ResponseDto<Object> response = new ResponseDto<object>();

        var user = new ApplicationUser()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.Email,
            NormalizedUserName = dto.Email.ToUpper(),
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToUpper(),
            PhoneNumber = dto.PhoneNumber
        };

        try
        {
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Register user - finished successfully");
            }
            else
            {
                _logger.LogInformation("Register user - result no succeeded:");
                var errorsMessage = "";
                foreach (var identityError in result.Errors)
                {
                    var currentErrorMessage =
                        $"Register user - error code: {identityError.Code}; description: {identityError.Description}. ";
                    errorsMessage += currentErrorMessage;
                    _logger.LogInformation(currentErrorMessage);
                }

                response.IsSuccess = false;
                response.Message = errorsMessage;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Register user - got exception: {e.Message}");

            response.IsSuccess = false;
            response.Message = e.Message;
            return response;
        }
        return response;
    }
    
    public async Task<LoginResponseDto> LoginUser(LoginUserDto dto)
    {
        
        _logger.LogInformation("Login user - start");

        var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.NormalizedEmail == dto.Email.ToUpper());

        if (user is  null)
        {
            _logger.LogInformation("Login user - user not found");
            return new LoginResponseDto()
            {
                User = null, Token = ""
            };
        }
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
        {
            _logger.LogInformation("Login user - incorrect password");
            return new LoginResponseDto()
            {
                User = null, Token = ""
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenGeneratorService.GenerateToken(user, roles);
        UserDto userDto = new()
        {
            Email = user.Email,
            ID = user.Id,
            Name = user.FirstName + " " + user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        var loginResponseDto = new LoginResponseDto()
        {
            User = userDto,
            Token = token
        };

        return loginResponseDto;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.NormalizedEmail == email.ToUpper());
        if (user is null)
        {
            return false;
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        await _userManager.AddToRoleAsync(user, roleName);
        return true;

    }
}