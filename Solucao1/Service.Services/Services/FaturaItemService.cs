using AutoMapper;
using Microsoft.AspNetCore.Http;
using Service.Business.Interfaces;
using Service.Commons.Notifications;
using Service.Commons.Notifications.Interfaces;
using Service.Entities.Models;
using Service.Entities.Requests;
using Service.Persistence.Repositories.Interfaces;
using Service.Persistence.UnitOfWork.Interfaces;
using Service.Services.Interfaces;

namespace Service.Services.Services;

public class FaturaItemService(
    IUnitOfWork _unitOfWork,
    INotificationContext _notificationContext,
    IFaturaItemBusiness _faturaItemBusiness,
    IFaturaItemRepository _faturaItemRepository,
    IFaturaRepository _faturaRepository,
    IFaturaBusiness _faturaBusiness
) : IFaturaItemService
{
    public async Task<bool> CreateAsync(int faturaId, FaturaItemCreateRequest request, int order)
    {
        var isNegative = _faturaItemBusiness.IsNegative(request.Valor);
        if (isNegative)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.FaturaItem.InvalidValue
            );
            return false;
        }
        
        var checkValueAndVerify = _faturaItemBusiness.CheckValueAndVerify(request);
        if (!checkValueAndVerify)
        {
            _notificationContext.SetDetails(
               statusCode: StatusCodes.Status400BadRequest,
               title: NotificationTitle.BadRequest,
               detail: NotificationMessage.FaturaItem.NotChecked
           );
            return false;
        }


        var record = new FaturaItemModel(faturaId, order * 10, request.Valor, request.Descricao);
        await _faturaItemRepository.AddAsync(record);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> AddToFaturaAsync(int faturaId, FaturaItemAddToFaturaRequest request)
    {
        var isNegative = _faturaItemBusiness.IsNegative(request.Valor);
        if (isNegative)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.FaturaItem.InvalidValue
            );
            return false;
        }

        var fatura = await _faturaRepository.GetByIdWithItemsAsync(faturaId);
        if (fatura is null)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status404NotFound,
                title: NotificationTitle.NotFound,
                detail: NotificationMessage.Fatura.NotFound
            );
            return false;
        }

        var isOverDue = _faturaBusiness.IsOverDue(fatura);
        if (isOverDue)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.IsOverDue
            );
            return false;
        }

        var isClosed = _faturaBusiness.IsClosed(fatura);
        if (isClosed)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.IsClosed
            );
            return false;
        }

        _unitOfWork.BeginTransaction();

        var order = (fatura.Items.Count + 1) * 10;

        var record = new FaturaItemModel(faturaId, order, request.Valor, request.Descricao);
        await _faturaItemRepository.AddAsync(record);
        await _unitOfWork.CommitAsync();

        fatura.SetTotal(fatura.Total += record.Valor);
        _faturaRepository.Update(fatura);
        await _unitOfWork.CommitAsync();

        await _unitOfWork.CommitAsync(true);

        return true;
    }

    public async Task<bool> RemoveToFaturaAsync(int id, int faturaId)
    {
        var record = await _faturaItemRepository.GetByIdAndFaturaId(id, faturaId);
        if (record is null)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status404NotFound,
                title: NotificationTitle.NotFound,
                detail: NotificationMessage.FaturaItem.NotFound
            );
            return false;
        }

        var isOverDue = _faturaBusiness.IsOverDue(record.Fatura);
        if (isOverDue)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.IsOverDue
            );
            return false;
        }

        var isClosed = _faturaBusiness.IsClosed(record.Fatura);
        if (isClosed)
        {
            _notificationContext.SetDetails(
                statusCode: StatusCodes.Status400BadRequest,
                title: NotificationTitle.BadRequest,
                detail: NotificationMessage.Fatura.IsClosed
            );
            return false;
        }

        _unitOfWork.BeginTransaction();

        record.Fatura.SetTotal(record.Fatura.Total -= record.Valor);
        _faturaRepository.Update(record.Fatura);
        await _unitOfWork.CommitAsync();

        _faturaItemRepository.Remove(record);
        await _unitOfWork.CommitAsync();

        await _unitOfWork.CommitAsync(true);

        return true;
    }
}
