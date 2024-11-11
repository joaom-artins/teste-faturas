using Service.Entities.Models;

namespace Service.Business.Interfaces;
public interface IFaturaBusiness
{
    bool IsValidDueDate(DateOnly date);
    bool IsClosed(FaturaModel record);
    bool IsOverDue(FaturaModel record);
}
