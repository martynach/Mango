namespace Mango.Services.ShoppingCartAPI.Models.Dto.Upsert;

public class UpsertCartDto
{
    public UpsertCartHeaderDto CartHeader { get; set; }
    public IEnumerable<UpsertCartDetailsDto> CartDetails { get; set; }
}