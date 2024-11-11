using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Service.Business.Interfaces;
using Service.Commons.Notifications;
using Service.Commons.Notifications.Interfaces;
using Service.Entities.Models;
using Service.Entities.Requests;
using Service.Persistence.Repositories.Interfaces;
using Service.Persistence.UnitOfWork.Interfaces;
using Service.Services.Interfaces;
using Service.Services.Services;

namespace Service.test.Tests;

public class FaturaServiceTest
{
    private readonly Mock<INotificationContext> _notificationContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFaturaRepository> _faturaRepositoryMock;
    private readonly Mock<IFaturaBusiness> _faturaBusinessMock;
    private readonly Mock<IFaturaItemService> _faturaItemServiceMock;
    private readonly FaturaService _faturaService;

    public FaturaServiceTest()
    {
        _notificationContextMock = new Mock<INotificationContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _faturaRepositoryMock = new Mock<IFaturaRepository>();
        _faturaBusinessMock = new Mock<IFaturaBusiness>();
        _faturaItemServiceMock = new Mock<IFaturaItemService>();

        _faturaService = new FaturaService(
            _notificationContextMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _faturaRepositoryMock.Object,
            _faturaBusinessMock.Object,
            _faturaItemServiceMock.Object
        );
    }

    #region Create tests

    [Fact]
    public async Task Createasync_ReturnsBadRequest_WhenDueDateIsInvalid()
    {
        var request = new FaturaCreateRequest
        {
            Cliente = "Cliente Teste",
            Vencimento = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            Itens = new[] { new FaturaItemCreateRequest { Descricao = "Item 1", Valor = 100 } }
        };

        _faturaBusinessMock.Setup(b => b.IsValidDueDate(request.Vencimento)).Returns(false);

        var result = await _faturaService.Createasync(request);

        Assert.False(result);
        _notificationContextMock.Verify(nc => nc.SetDetails(
            StatusCodes.Status400BadRequest,
            NotificationTitle.BadRequest,
            NotificationMessage.Fatura.InvalidDueDate
        ), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenItensAreEmpty()
    {
        var request = new FaturaCreateRequest
        {
            Cliente = "Cliente Teste",
            Vencimento = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
            Itens = Array.Empty<FaturaItemCreateRequest>()
        };

        _notificationContextMock.Setup(nc => nc.HasNotifications).Returns(true);
        _notificationContextMock.Setup(nc => nc.StatusCode).Returns(StatusCodes.Status400BadRequest);
        _notificationContextMock.Setup(nc => nc.Title).Returns("Requisição inválida");
        _notificationContextMock.Setup(nc => nc.Detail).Returns("A fatura deve ter pelo menos um item.");

        var result = await _faturaService.Createasync(request);

        Assert.False(result);
        _notificationContextMock.Verify(nc => nc.SetDetails(
            StatusCodes.Status400BadRequest,
            NotificationTitle.BadRequest,
            NotificationMessage.Fatura.HasOneItem
        ), Times.Once);
    }

    #endregion

    #region Close tests

    [Fact]
    public async Task CloseAsync_ReturnsFalse_WhenFaturaNotFound()
    {
        var faturaId = 1;
        _faturaRepositoryMock.Setup(repo => repo.GetByIdAsync(faturaId)).ReturnsAsync((FaturaModel?)null);

        var result = await _faturaService.CloseAsync(faturaId);

        Assert.False(result);
        _notificationContextMock.Verify(n => n.SetDetails(
            StatusCodes.Status404NotFound,
            NotificationTitle.NotFound,
            NotificationMessage.Fatura.NotFound
        ), Times.Once);
    }

    [Fact]
    public async Task CloseAsync_ReturnsFalse_WhenFaturaAlreadyClosed()
    {
        var faturaId = 1;
        var fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now)) { Fechada = true };
        _faturaRepositoryMock.Setup(repo => repo.GetByIdAsync(faturaId)).ReturnsAsync(fatura);

        var result = await _faturaService.CloseAsync(faturaId);

        Assert.False(result);
        _notificationContextMock.Verify(n => n.SetDetails(
            StatusCodes.Status400BadRequest,
            NotificationTitle.BadRequest,
            NotificationMessage.Fatura.AlreadyIsClosed
        ), Times.Once);
    }

    [Fact]
    public async Task CloseAsync_SuccessfullyClosesFatura()
    {
        var faturaId = 1;
        var fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now)) { Fechada = false };

        _faturaRepositoryMock.Setup(repo => repo.GetByIdAsync(faturaId)).ReturnsAsync(fatura);
        _faturaRepositoryMock.Setup(repo => repo.Update(It.IsAny<FaturaModel>())).Verifiable();

        var result = await _faturaService.CloseAsync(faturaId);

        Assert.True(result);
        _faturaRepositoryMock.Verify(repo => repo.Update(fatura), Times.Once);
    }

    #endregion

    #region Delete tests

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenFaturaNotFound()
    {
        var faturaId = 1;
        _faturaRepositoryMock.Setup(r => r.GetByIdAsync(faturaId)).ReturnsAsync((FaturaModel?)null);

        var result = await _faturaService.DeleteAynsc(faturaId);

        Assert.False(result);
        _notificationContextMock.Verify(n => n.SetDetails(
            StatusCodes.Status404NotFound,
            NotificationTitle.NotFound,
            NotificationMessage.Fatura.NotFound
        ), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenFaturaIsDeletedSuccessfully()
    {
        var faturaId = 1;
        var fatura = new FaturaModel("cliente", DateOnly.FromDateTime(DateTime.Now.AddDays(5)));

        _faturaRepositoryMock.Setup(r => r.GetByIdAsync(faturaId)).ReturnsAsync(fatura);

        _faturaRepositoryMock.Setup(r => r.Remove(It.IsAny<FaturaModel>())).Verifiable();

        var result = await _faturaService.DeleteAynsc(faturaId);

        Assert.True(result);
        _faturaRepositoryMock.Verify(r => r.Remove(It.IsAny<FaturaModel>()), Times.Once);
    }

    #endregion
}
