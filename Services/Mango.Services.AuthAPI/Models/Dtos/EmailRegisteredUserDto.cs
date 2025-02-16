namespace Mango.Services.AuthAPI.Models.Dtos;

public class EmailRegisteredUserDto
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
    
    public string RoleName { get; set; }
}