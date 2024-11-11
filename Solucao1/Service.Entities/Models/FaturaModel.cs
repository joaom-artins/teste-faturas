using System.ComponentModel.DataAnnotations;

namespace Service.Entities.Models;

public class FaturaModel
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Client { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public double Total { get; set; }
    public DateOnly Vencimento { get; set; }
    public bool Fechada { get; set; } = false;

    public ICollection<FaturaItemModel> Items { get; set; } = default!;

    public FaturaModel(string client, DateOnly vencimento)
    {
        SetClient(client);
        SetVencimento(vencimento);
    }


    public void SetClient(string client)
    {
        Client = client;
    }

    public void SetVencimento(DateOnly vencimento)
    {
        Vencimento = vencimento;
    }

    public void SetTotal(double total)
    {
        Total = total;
    }

    public void SetFechada(bool fechada)
    {
        Fechada = fechada;
    }
}
