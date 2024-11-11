using Service.Entities.Models;

namespace Service.Persistence.Repositories.Interfaces;

public interface IFaturaRepository : IGenericRepository<FaturaModel>
{
    Task<IEnumerable<FaturaModel>> GetAllWithItemsAsync();
    Task<FaturaModel?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<FaturaModel>> Get10MoreExpensiveAsync();
    Task<double> GetTotalByClientAsync(string client);
    Task<double> GetTotalByYearAndMonth(int year, int month);
}
