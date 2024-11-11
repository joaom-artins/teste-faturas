using Service.Entities.Requests;

namespace Service.Services.Interfaces;

public interface IFaturaItemService
{
    Task<bool> CreateAsync(int faturaId, FaturaItemCreateRequest request, int order);
    Task<bool> AddToFaturaAsync(int faturaId, FaturaItemAddToFaturaRequest request);
    Task<bool> RemoveToFaturaAsync(int id, int faturaId);
}
