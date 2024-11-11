using Service.Entities.Models;
using Service.Entities.Responses;

namespace Service.Services.Interfaces;

public interface IFaturaManagementService
{
    Task<IEnumerable<FaturaFaturaManagementGetAllResponse>> GetAllAsync();
    Task<IEnumerable<FaturaItemModel>> Get10ItemsMoreExpensiveAsync();
    Task<IEnumerable<FaturaModel>> Get10FaturasMoreExpensiveAsync();
    Task<FaturaManagementGetTotalByClientResponse> GetTotalByClientAsync(string client);
    Task<FaturaManagementGetTotalByYearAndMonthResponse> GetTotalByYearAndMonthAsync(int year, int month);
}
