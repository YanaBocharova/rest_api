using AutoMapper;
using Domain.Entity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Persistence.Repositories;
using Services;
using Services.Abstract.Dto;
using Services.Abstract.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace WebApiTests.ServicesTests
{
    [TestClass]
    public class AccountsServiceTests
    {
        private Mock<IUnitOfWork> mockUow;
        private Mock<IRepository<Account>> mockRepo;
        private Mock<IMapper> mockMapper;
        private IAccountsService service;
        private CancellationToken token;

        [TestInitialize]
        public void Setup()
        {
            mockUow = new Mock<IUnitOfWork>();
            mockRepo = new Mock<IRepository<Account>>();
            mockMapper = new Mock<IMapper>();
            token = CancellationToken.None;

            mockUow.SetupGet(x => x.AccountsRepository)
                   .Returns(mockRepo.Object);

            service = new AccountsService(mockUow.Object, mockMapper.Object);
        }

        [TestMethod]
        public async Task GetAccountByEmail_WithPassword_ShouldReturnNull_WhenPasswordIsWrong()
        {
            // Arrange
            var account = new Account
            {
                Email = "a@b.com",
                Password = new PasswordHasher<Account>()
                    .HashPassword(null, "correct")
            };

            mockRepo.Setup(r => r.GetAsync("a@b.com", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(account);

            mockMapper.Setup(m => m.Map<AccountDto>(It.IsAny<Account>()))
                      .Returns(new AccountDto { Email = "a@b.com" });

            // Act
            var result = await service.GetAccountByEmail("a@b.com", "wrong_password", token);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAccountByEmail_WithPassword_ShouldReturnAccount_WhenPasswordIsCorrect()
        {
            var hasher = new PasswordHasher<Account>();
            var account = new Account
            {
                Email = "a@b.com",
                Password = hasher.HashPassword(null, "correct")
            };

            mockRepo.Setup(r => r.GetAsync("a@b.com", It.IsAny<CancellationToken>()))
                    .ReturnsAsync(account);

            mockMapper.Setup(m => m.Map<AccountDto>(account))
                      .Returns(new AccountDto { Email = "a@b.com" });

            var result = await service.GetAccountByEmail("a@b.com", "correct", token);

            Assert.IsNotNull(result);
            Assert.AreEqual("a@b.com", result.Email);
        }

        [TestMethod]
        public async Task CreateAccount_ShouldCallRepositoryWithMappedEntity()
        {
            var dto = new AccountDto
            {
                Email = "new@test.com",
                Password = "pwd"
            };

            mockMapper.Setup(m => m.Map<Account>(dto))
                      .Returns(new Account { Email = dto.Email, Password = dto.Password });

            mockMapper.Setup(m => m.Map<AccountDto>(It.IsAny<Account>()))
                      .Returns(new AccountDto { Email = dto.Email });

            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            mockRepo.Setup(r => r.GetAsync(dto.Email, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Account)null);

            await service.CreateAccount(dto, token);

            mockRepo.Verify(r => r.CreateAsync(
                It.Is<Account>(a => a.Email == "new@test.com"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task RemoveAccountByEmail_ShouldCallRemoveAndSaveChanges()
        {
            // Arrange
            mockRepo.Setup(r => r.RemoveAsync("x@y.com", It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);

            mockRepo.Setup(r => r.GetAsync("x@y.com", It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Account)null);

            mockMapper.Setup(m => m.Map<AccountDto>(null))
                      .Returns((AccountDto)null);

            // Act
            var result = await service.RemoveAccountByEmail("x@y.com", token);

            // Assert
            mockRepo.Verify(r => r.RemoveAsync("x@y.com", It.IsAny<CancellationToken>()), Times.Once);
            mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsTrue(result);
        }
    }
}