namespace Service.Entities.Responses;

public class FaturaItemsFaturaManagementGetAllResponse
{
    public int Id { get; private set; }
    public int FaturaId { get; private set; }
    public int Ordem { get; private set; }
    public double Valor { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
}
