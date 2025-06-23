using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.Utilisateurs.LoginUser;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Utilisateurs.Events;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LoginUserCommandHandler _handler;
        private readonly Mock<ILogger<LoginUserCommandHandler>> _mockLogger;


        public LoginUserCommandHandlerTests()
        {
            // Set up a dummy IUserStore for UserManager
            var userStoreMock = new Mock<IUserStore<IdentityUser>>().Object;
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                userStoreMock,
                null,  // IOptions<IdentityOptions>
                null,  // IPasswordHasher<IdentityUser>
                null,  // IEnumerable<IUserValidator<IdentityUser>>
                null,  // IEnumerable<IPasswordValidator<IdentityUser>>
                null,  // ILookupNormalizer
                null,  // IdentityErrorDescriber
                null,  // IServiceProvider
                null   // ILogger<UserManager<IdentityUser>>
            );

            // Set up HttpContextAccessor and ClaimFactory required by SignInManager
            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null, // IOptions<IdentityOptions>
                null, // ILogger<SignInManager<IdentityUser>>
                null, // IAuthenticationSchemeProvider
                null  // IUserConfirmation<IdentityUser>
            );

            _mockMediator = new Mock<IMediator>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<LoginUserCommandHandler>>();


            _handler = new LoginUserCommandHandler(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockMediator.Object,
                _mockTokenService.Object,
                _mockLogger.Object

            );
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new LoginDto("alice", "wrongPass");
            var command = new LoginUserCommand(dto);

            _mockUserManager
                .Setup(x => x.FindByNameAsync("alice"))
                .ReturnsAsync((IdentityUser)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Identifiant ou mot de passe incorrect.");
        }

        [Fact]
        public async Task Handle_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new LoginDto("bob", "badPass");
            var command = new LoginUserCommand(dto);

            var identityUser = new IdentityUser { Id = Guid.NewGuid().ToString(), UserName = "bob" };

            _mockUserManager
                .Setup(x => x.FindByNameAsync("bob"))
                .ReturnsAsync(identityUser);

            _mockSignInManager
                .Setup(x => x.CheckPasswordSignInAsync(identityUser, "badPass", false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Identifiant ou mot de passe incorrect.");
        }

        [Fact]
        public async Task Handle_UserNameNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new LoginDto("charlie", "anyPass");
            var command = new LoginUserCommand(dto);

            var identityUser = new IdentityUser { Id = Guid.NewGuid().ToString(), UserName = null! };

            _mockUserManager
                .Setup(x => x.FindByNameAsync("charlie"))
                .ReturnsAsync(identityUser);

            _mockSignInManager
                .Setup(x => x.CheckPasswordSignInAsync(identityUser, "anyPass", false))
                .ReturnsAsync(SignInResult.Success);

            _mockUserManager
                .Setup(x => x.GetRolesAsync(identityUser))
                .ReturnsAsync(new List<string>()); // no roles needed

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<InvalidOperationException>()
                     .WithMessage("Nom d'utilisateur introuvable.");
        }

        [Fact]
        public async Task Handle_ValidCredentials_PublishesEventAndReturnsToken()
        {
            // Arrange
            var dto = new LoginDto("david", "correctPass");
            var command = new LoginUserCommand(dto);

            var userId = Guid.NewGuid().ToString();
            var identityUser = new IdentityUser { Id = userId, UserName = "david" };

            _mockUserManager
                .Setup(x => x.FindByNameAsync("david"))
                .ReturnsAsync(identityUser);

            _mockSignInManager
                .Setup(x => x.CheckPasswordSignInAsync(identityUser, "correctPass", false))
                .ReturnsAsync(SignInResult.Success);

            var roles = new List<string> { "Admin", "Enseignant" };
            _mockUserManager
                .Setup(x => x.GetRolesAsync(identityUser))
                .ReturnsAsync(roles);

            var generatedToken = "jwt-token-123";
            _mockTokenService
                .Setup(x => x.GenerateToken(Guid.Parse(userId), "david", roles))
                .Returns(generatedToken);

            var publishCount=0;

            _mockMediator
                .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Callback(() => publishCount++)
                .Returns(Task.CompletedTask);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Token.Should().Be(generatedToken);
            response.Identifiant.Should().Be("david");
            response.Roles.Should().BeEquivalentTo(roles);
            
            publishCount.Should().Be(1);

          }
    }
}


