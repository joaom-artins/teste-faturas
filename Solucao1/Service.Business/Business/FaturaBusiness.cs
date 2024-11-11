using Service.Business.Interfaces;
using Service.Entities.Models;

namespace Service.Business.Business;

public class FaturaBusiness : IFaturaBusiness
{
    public bool IsValidDueDate(DateOnly date)
    {
        if (DateOnly.FromDateTime(DateTime.Now) >= date)
        {
            return false;
        }

        return true;
    }

    public bool IsClosed(FaturaModel record)
    {
        if (!record.Fechada)
        {
            return false;
        }

        return true;
    }

    public bool IsOverDue(FaturaModel record)
    {
        if (record.Vencimento > DateOnly.FromDateTime(DateTime.Now))
        {
            return false;
        }

        return true;
    }
}
