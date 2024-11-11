using System.ComponentModel.DataAnnotations;

namespace Service.Entities.Models;

public class FaturaItemModel
{
    [Key]
    public int Id { get; private set; }
    public int FaturaId { get; private set; }
    public FaturaModel Fatura { get;  set; } = default!;
    public int Ordem { get; private set; }
    public double Valor { get; private set; }
    [MaxLength(20)]
    public string Descricao { get; private set; } = string.Empty;

    public FaturaItemModel(int faturaId, int ordem, double valor,string descricao)
    {
        SetFaturaId(faturaId);
        SetOrdem(ordem);
        SetValor(valor);
        SetDescricao(descricao);
    }

    public void SetFaturaId(int faturaId)
    {
        FaturaId = faturaId;
    }

    public void SetOrdem(int ordem)
    {
        Ordem = ordem;
    }

    public void SetValor(double valor)
    {
        Valor = valor;
    }

    public void SetDescricao(string descricao)
    {
        Descricao = descricao;
    }
}
