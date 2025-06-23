using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.Utilisateurs.CreateUser;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs;
using EduTrackOne.Domain.Utilisateurs.Events;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<CreateUserCommandHandler>> _mockLogger;
        private readonly IPasswordHasher<Utilisateur> _passwordHasher;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();

            // Fournir une vraie implémentation de PasswordHasher pour le handler
            _passwordHasher = new PasswordHasher<Utilisateur>();

            _handler = new CreateUserCommandHandler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockCurrentUser.Object,
                _mockMediator.Object,
                _mockLogger.Object,
                _passwordHasher
            );
        }

        [Fact]
        public async Task Handle_NonAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new CreateUserDto(
                Identifiant: "nouvelUser",
                MotDePasse: "MotDePasse123",
                Role: UserRoleDto.Enseignant,
                Statut: (UserStatusDto)StatutUtilisateur.StatutEnum.Actif,
                Email: "user@example.com"
            );
            var command = new CreateUserCommand(dto);

            // Simuler CurrentUser non-admin
            _mockCurrentUser
                .Setup(x => x.IsInRole(It.IsAny<RoleUtilisateur.Role>()))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Only admins can create users.");

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Utilisateur>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Admin_CreatesUser_SavesAndPublishesEvent()
        {
            // Arrange
            var dto = new CreateUserDto(
                Identifiant: "nouvelUser",
                MotDePasse: "MotDePasse123",
                Role: UserRoleDto.Enseignant,
                Statut: (UserStatusDto)StatutUtilisateur.StatutEnum.Actif,
                Email: "user@example.com"
            );
            var command = new CreateUserCommand(dto);

            // Simuler CurrentUser admin
            _mockCurrentUser
                .Setup(x => x.IsInRole(RoleUtilisateur.Role.Admin))
                .Returns(true);

            Utilisateur? capturedUser = null;
            _mockRepo
                .Setup(r => r.AddAsync(It.IsAny<Utilisateur>(), It.IsAny<CancellationToken>()))
                .Callback<Utilisateur, CancellationToken>((u, _) => capturedUser = u)
                .Returns(Task.CompletedTask);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var publishedEvents = new List<INotification>();
            _mockMediator
                .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Callback<INotification, CancellationToken>((evt, _) => publishedEvents.Add(evt))
                .Returns(Task.CompletedTask);

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert

            // 1. Un utilisateur a bien été capturé et l’ID retourné correspond
            capturedUser.Should().NotBeNull();
            resultId.Should().Be(capturedUser!.Id);

            // 2. Vérifier les propriétés business de l’utilisateur créé
            capturedUser.Identifiant.Should().Be("nouvelUser");
            capturedUser.Role.Valeur.Should().Be(RoleUtilisateur.Role.Enseignant);
            capturedUser.Statut.Value.Should().Be(StatutUtilisateur.StatutEnum.Actif);
            capturedUser.Email.Value.Should().Be("user@example.com");

            // 3. Vérifier qu’un hash de mot de passe a été généré (non vide et différent du mot en clair)
            capturedUser.MotDePasseHash.Should().NotBeNullOrWhiteSpace();
            capturedUser.MotDePasseHash.Should().NotBe("MotDePasse123");

            // 4. AddAsync et SaveChangesAsync ont été appelés une seule fois
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Utilisateur>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // 5. Un événement UserCreatedEvent a été publié
            publishedEvents.Should().HaveCount(1);
            publishedEvents[0].Should().BeOfType<UserCreatedEvent>();

            var evt = (UserCreatedEvent)publishedEvents[0];
            evt.UserId.Should().Be(capturedUser.Id);
            evt.Identifiant.Should().Be("nouvelUser");
            evt.Email.Should().Be("user@example.com");

            // 6. Après publication, DomainEvents de l’utilisateur sont vidés
            capturedUser.DomainEvents.Should().BeEmpty();
        }
    }
}
