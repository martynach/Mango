using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Mango.Services.ShoppingCartAPI.Utilities;

public class BackendApiAuthenticationHttpClientHandler: DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        
        // request.Headers.Add("Authorization", $"Bearer {token}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


        return await base.SendAsync(request, cancellationToken);
    }

    
}