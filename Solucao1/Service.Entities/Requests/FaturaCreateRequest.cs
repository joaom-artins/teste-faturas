namespace Service.Entities.Requests;

public class FaturaCreateRequest
{
    public string Cliente { get; set; } = string.Empty;
    public DateOnly Vencimento { get; set; }
    public IEnumerable<FaturaItemCreateRequest> Itens { get; set; } = default!;
}
