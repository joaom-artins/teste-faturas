using AutoMapper;
using Microsoft.AspNetCore.Http;
using Service.Commons.Notifications;
using Service.Commons.Notifications.Interfaces;
using Service.Entities.Models;
using Service.Entities.Responses;
using Service.Persistence.Repositories.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services.Services;

public class FaturaManagementService(
    INotificationContext _notificationContext,
    IMapper _mapper,
    IFaturaItemRepository _faturaItemRepository,
    IFaturaRepository _faturaRepository
) : IFaturaManagementService
{
    public async Task<IEnumerable<FaturaFaturaManagementGetAllResponse>> GetAllAsync()
    {
        var records = await _faturaRepository.GetAllWithItemsAsync();

        return _mapper.Map<IEnumerable<FaturaFaturaManagementGetAllResponse>>(records);
    }

    public async Task<IEnumerable<FaturaItemModel>> Get10ItemsMoreExpensiveAsync()
    {
        var records = await _faturaItemRepository.Get10MoreExpensives();

        return records;
    }

    public async Task<IEnumerable<FaturaModel>> Get10FaturasMoreExpensiveAsync()
    {
        var records = await _faturaRepository.Get10MoreExpensiveAsync();

        return records;
    }

    public async Task<FaturaManagementGetTotalByClientResponse> GetTotalByClientAsync(string client)
    {
        var total = await _faturaRepository.GetTotalByClientAsync(client);
        if (total == 0)
        {
            _notificationContext.SetDetails(
               statusCode: StatusCodes.Status400BadRequest,
               title: NotificationTitle.BadRequest,
               detail: NotificationMessage.Fatura.NotForClient
           );
            return default!;
        }

        return new FaturaManagementGetTotalByClientResponse
        {
            Total = total,
        };
    }

    public async Task<FaturaManagementGetTotalByYearAndMonthResponse> GetTotalByYearAndMonthAsync(int year, int month)
    {
        var total = await _faturaRepository.GetTotalByYearAndMonth(year, month);
        if (total == 0)
        {
            _notificationContext.SetDetails(
               statusCode: StatusCodes.Status400BadRequest,
               title: NotificationTitle.BadRequest,
               detail: NotificationMessage.Fatura.NotForDate
           );
            return default!;
        }

        return new FaturaManagementGetTotalByYearAndMonthResponse
        {
            Total = total,
        };
    }
}
