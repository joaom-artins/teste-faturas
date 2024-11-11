using Service.Entities.Models;
using Service.Entities.Requests;
using Service.Entities.Responses;

namespace Service.Services.Interfaces;

public interface IFaturaService
{
    Task<FaturaGetByIdResponse> GetByIdasync(int id);
    Task<bool> Createasync(FaturaCreateRequest request);
    Task<bool> CloseAsync(int id);
    Task<bool> DeleteAynsc(int id);
}
