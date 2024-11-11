using AutoMapper;
using Microsoft.AspNetCore.Http;
using Service.Business.Interfaces;
using Service.Commons.Notifications;
using Service.Commons.Notifications.Interfaces;
using Service.Entities.Models;
using Service.Entities.Requests;
using Service.Entities.Responses;
using Service.Persistence.Repositories.Interfaces;
using Service.Persistence.UnitOfWork.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services.Services;

public class FaturaService(
    INotificationContext _notificationContext,
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IFaturaRepository _faturaRepository,
    IFaturaBusiness _faturaBusiness,
    IFaturaItemService _faturaItemService
) : IFaturaService
{
    public async Task<FaturaGetByIdResponse> GetByIdasync(int id)
    {
        var record = await _faturaRepository.GetByIdWithItemsAsync(id);
        if (record is null)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status404NotFound,
                title: NotificationTitle.NotFound,
                detail: NotificationMessage.Fatura.NotFound
            );
            return default!; ;
        }

        return _mapper.Map<FaturaGetByIdResponse>(record);
    }

    public async Task<bool> Createasync(FaturaCreateRequest request)
    {
        if (!request.Itens.Any())
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.HasOneItem
            );
            return false;
        }

        var isValidDueDate = _faturaBusiness.IsValidDueDate(request.Vencimento);
        if (!isValidDueDate)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.InvalidDueDate
            );
            return false;
        }

        _unitOfWork.BeginTransaction();

        var record = new FaturaModel(request.Cliente, request.Vencimento);
        await _faturaRepository.AddAsync(record);
        await _unitOfWork.CommitAsync();

        var count = 1;
        foreach (var item in request.Itens)
        {
            await _faturaItemService.CreateAsync(record.Id, item, count);
            count++;
        }

        record.SetTotal(request.Itens.Sum(x => x.Valor));
        _faturaRepository.Update(record);
        await _unitOfWork.CommitAsync();

        await _unitOfWork.CommitAsync(true);

        return true;
    }

    public async Task<bool> CloseAsync(int id)
    {
        var record = await _faturaRepository.GetByIdAsync(id);
        if (record is null)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status404NotFound,
                title: NotificationTitle.NotFound,
                detail: NotificationMessage.Fatura.NotFound
            );
            return false;
        }

        if (record.Fechada)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.AlreadyIsClosed
            );
            return false;
        }

        record.SetFechada(true);
        _faturaRepository.Update(record);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteAynsc(int id)
    {
        var record = await _faturaRepository.GetByIdAsync(id);
        if (record is null)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status404NotFound,
                title: NotificationTitle.NotFound,
                detail: NotificationMessage.Fatura.NotFound
            );
            return false;
        }

        _faturaRepository.Remove(record);
        await _unitOfWork.CommitAsync();

        return true;
    }
}
