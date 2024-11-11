namespace Service.Entities.Requests;

public class FaturaItemCreateRequest
{
    public double Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool VerificaValor { get; set; }
}
