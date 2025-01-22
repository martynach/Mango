namespace Mango.Web.Service.IService;

public interface ITokenProviderService
{
    public string? GetToken();
    public void SetToken(string token);
    public void CleanToken();
}