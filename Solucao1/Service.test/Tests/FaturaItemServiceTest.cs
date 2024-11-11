using Moq;
using Service.Business.Interfaces;
using Service.Commons.Notifications.Interfaces;
using Service.Entities.Models;
using Service.Entities.Requests;
using Service.Persistence.Repositories.Interfaces;
using Service.Persistence.UnitOfWork.Interfaces;
using Service.Services.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using Service.Commons.Notifications;

namespace Service.Tests
{
    public class FaturaItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<INotificationContext> _notificationContextMock;
        private readonly Mock<IFaturaItemBusiness> _faturaItemBusinessMock;
        private readonly Mock<IFaturaItemRepository> _faturaItemRepositoryMock;
        private readonly Mock<IFaturaRepository> _faturaRepositoryMock;
        private readonly Mock<IFaturaBusiness> _faturaBusinessMock;
        private readonly FaturaItemService _faturaItemService;

        public FaturaItemServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _notificationContextMock = new Mock<INotificationContext>();
            _faturaItemBusinessMock = new Mock<IFaturaItemBusiness>();
            _faturaItemRepositoryMock = new Mock<IFaturaItemRepository>();
            _faturaRepositoryMock = new Mock<IFaturaRepository>();
            _faturaBusinessMock = new Mock<IFaturaBusiness>();

            _faturaItemService = new FaturaItemService(
                _unitOfWorkMock.Object,
                _notificationContextMock.Object,
                _faturaItemBusinessMock.Object,
                _faturaItemRepositoryMock.Object,
                _faturaRepositoryMock.Object,
                _faturaBusinessMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenItemValueIsNegative()
        {
            var faturaId = 1;
            var request = new FaturaItemCreateRequest
            {
                Descricao = "Item Teste",
                Valor = -100,
                VerificaValor = true
            };

            _faturaItemBusinessMock.Setup(b => b.IsNegative(request.Valor)).Returns(true);

            var result = await _faturaItemService.CreateAsync(faturaId, request, 1);

            Assert.False(result);

            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status400BadRequest,
                NotificationTitle.BadRequest,
                NotificationMessage.FaturaItem.InvalidValue
            ), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenItemCheckFails()
        {
            var faturaId = 1;
            var request = new FaturaItemCreateRequest { Descricao = "Item Teste", Valor = 1001 };

            _faturaItemBusinessMock.Setup(b => b.CheckValueAndVerify(request)).Returns(false);

            var result = await _faturaItemService.CreateAsync(faturaId, request, 1);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status400BadRequest,
                NotificationTitle.BadRequest,
                NotificationMessage.FaturaItem.NotChecked
            ), Times.Once);
        }

        #endregion

        #region AddToFaturaAsync Tests

        [Fact]
        public async Task AddToFaturaAsync_ReturnsBadRequest_WhenFaturaNotFound()
        {
            var faturaId = 1;
            var request = new FaturaItemAddToFaturaRequest { Descricao = "Novo Item", Valor = 200 };

            _faturaRepositoryMock.Setup(r => r.GetByIdWithItemsAsync(faturaId)).ReturnsAsync((FaturaModel?)null);

            var result = await _faturaItemService.AddToFaturaAsync(faturaId, request);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status404NotFound,
                NotificationTitle.NotFound,
                NotificationMessage.Fatura.NotFound
            ), Times.Once);
        }

        [Fact]
        public async Task AddToFaturaAsync_ReturnsBadRequest_WhenFaturaIsOverDue()
        {
            var faturaId = 1;
            var request = new FaturaItemAddToFaturaRequest { Descricao = "Novo Item", Valor = 200 };

            var fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now.AddDays(-10)));
            _faturaRepositoryMock.Setup(r => r.GetByIdWithItemsAsync(faturaId)).ReturnsAsync(fatura);
            _faturaBusinessMock.Setup(b => b.IsOverDue(It.IsAny<FaturaModel>())).Returns(true);

            var result = await _faturaItemService.AddToFaturaAsync(faturaId, request);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status400BadRequest,
                NotificationTitle.BadRequest,
                NotificationMessage.Fatura.IsOverDue
            ), Times.Once);
        }

        [Fact]
        public async Task AddToFaturaAsync_ReturnsBadRequest_WhenFaturaIsClosed()
        {
            var faturaId = 1;
            var request = new FaturaItemAddToFaturaRequest { Descricao = "Novo Item", Valor = 200 };

            var fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now.AddDays(10))) { Fechada = true };
            _faturaRepositoryMock.Setup(r => r.GetByIdWithItemsAsync(faturaId)).ReturnsAsync(fatura);
            _faturaBusinessMock.Setup(b => b.IsClosed(It.IsAny<FaturaModel>())).Returns(true);

            var result = await _faturaItemService.AddToFaturaAsync(faturaId, request);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status400BadRequest,
                NotificationTitle.BadRequest,
                NotificationMessage.Fatura.IsClosed
            ), Times.Once);
        }

        #endregion

        #region RemoveToFaturaAsync Tests

        [Fact]
        public async Task RemoveToFaturaAsync_ReturnsBadRequest_WhenItemNotFound()
        {
            var itemId = 1;
            var faturaId = 1;

            _faturaItemRepositoryMock.Setup(r => r.GetByIdAndFaturaId(itemId, faturaId)).ReturnsAsync((FaturaItemModel?)null);

            var result = await _faturaItemService.RemoveToFaturaAsync(itemId, faturaId);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status404NotFound,
                NotificationTitle.NotFound,
                NotificationMessage.FaturaItem.NotFound
            ), Times.Once);
        }

        [Fact]
        public async Task RemoveToFaturaAsync_ReturnsBadRequest_WhenFaturaIsOverDue()
        {
            var itemId = 1;
            var faturaId = 1;

            var faturaItem = new FaturaItemModel(faturaId, 10, 200, "Item Teste");
            var fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now.AddDays(-10)));

            _faturaItemRepositoryMock.Setup(r => r.GetByIdAndFaturaId(itemId, faturaId))
                .ReturnsAsync(new FaturaItemModel(faturaId, 10, 200, "Item Teste")
                {
                    Fatura = new FaturaModel("Cliente A", DateOnly.FromDateTime(DateTime.Now.AddDays(-10)))
                });
            _faturaBusinessMock.Setup(b => b.IsOverDue(It.IsAny<FaturaModel>())).Returns(true); 

            var result = await _faturaItemService.RemoveToFaturaAsync(itemId, faturaId);

            Assert.False(result);
            _notificationContextMock.Verify(nc => nc.SetDetails(
                StatusCodes.Status400BadRequest,
                NotificationTitle.BadRequest,
                NotificationMessage.Fatura.IsOverDue
            ), Times.Once);
        }

        #endregion
    }
}
