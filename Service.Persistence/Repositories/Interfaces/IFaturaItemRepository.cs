using Service.Entities.Models;

namespace Service.Persistence.Repositories.Interfaces;

public interface IFaturaItemRepository : IGenericRepository<FaturaItemModel>
{
    Task<FaturaItemModel?> GetByIdAndFaturaId(int id, int faturaId);
    Task<IEnumerable<FaturaItemModel>> Get10MoreExpensives();
}
