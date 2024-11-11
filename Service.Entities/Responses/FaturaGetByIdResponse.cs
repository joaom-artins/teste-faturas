namespace Service.Entities.Responses;

public class FaturaGetByIdResponse
{
    public int Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public double Total { get; set; }
    public DateOnly Vencimento { get; set; }
    public bool Fechada { get; set; } = false;
    public ICollection<FaturaItemFaturaGetByIdResponse> Items { get; set; } = default!;
}
