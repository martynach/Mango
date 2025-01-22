using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProviderService _tokenProviderService;

    public AuthController(IAuthService authService, ITokenProviderService tokenProviderService)
    {
        _authService = authService;
        _tokenProviderService = tokenProviderService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var loginUserDto = new LoginUserDto();
        return View(loginUserDto);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var result = await _authService.LoginAsync(loginUserDto);
        // result = null;
        if (result != null && result.IsSuccess)
        {
            LoginResponseDto loginResponseDto =
                JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(result.Result));

            await SignInUser(loginResponseDto.Token);
            _tokenProviderService.SetToken(loginResponseDto.Token);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("CustomError", result?.Message ?? "Some errors occured");

        // return View();
        return View(loginUserDto);
    }

    public IActionResult Register()
    {
        var roleList = new List<SelectListItem>()
        {
            new SelectListItem(SD.RoleAdmin, SD.RoleAdmin),
            new SelectListItem(SD.RoleCustomer, SD.RoleCustomer)
        };

        ViewBag.RoleList = roleList;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var result = await _authService.RegisterAsync(registerUserDto);
        if (result != null && result.IsSuccess)
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.RoleName))
            {
                registerUserDto.RoleName = SD.RoleCustomer;
            }

            var assignRoleResponse = await _authService.AssignRoleAsync(registerUserDto);

            if (assignRoleResponse != null && assignRoleResponse.IsSuccess)
            {
                TempData["success"] = "Registration successful";
                return RedirectToAction(nameof(Login));
            }
        }
 

        var roleList = new List<SelectListItem>()
        {
            new SelectListItem(SD.RoleAdmin, SD.RoleAdmin),
            new SelectListItem(SD.RoleCustomer, SD.RoleCustomer)
        };

        ViewBag.RoleList = roleList;
        TempData["error"] = "Registration failed" + (result?.Message ?? "");

        return View(registerUserDto);
    }

    public async Task<IActionResult> Logout()
    {
        _tokenProviderService.CleanToken();
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // public IActionResult Logout()
    // {
    //     return View();
    // }

    private async Task SignInUser(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);


        var claimsIdentity = new ClaimsIdentity("password");
        claimsIdentity.AddClaims(jwtSecurityToken.Claims);

        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name,
            jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? ""));

        var userPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
    }
}