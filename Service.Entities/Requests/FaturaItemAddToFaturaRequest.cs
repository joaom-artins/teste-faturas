namespace Service.Entities.Requests;

public class FaturaItemAddToFaturaRequest
{
    public double Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool VerificaValor { get; set; }
}
