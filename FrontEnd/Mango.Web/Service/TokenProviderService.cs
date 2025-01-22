using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service;

public class TokenProviderService: ITokenProviderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenProviderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    


    public string? GetToken()
    {
        string? token = null;
        // var hasToken = _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
        // return (hasToken ?? false) ? token : ""; if we want return type to be string
        
        _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
        return token;
    }


    public void SetToken(string token)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);

    }
    
    public void CleanToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        
    }
}