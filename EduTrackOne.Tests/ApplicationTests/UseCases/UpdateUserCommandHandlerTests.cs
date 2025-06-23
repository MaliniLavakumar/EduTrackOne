using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.Utilisateurs.UpdateUser;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Domain.Utilisateurs;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduTrackOne.Application.Classes.AddInscription;
using Microsoft.Extensions.Logging;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<IMediator> _mockMediator;
        private readonly UpdateUserCommandHandler _handler; 
        private readonly Mock<ILogger<UpdateUserCommandHandler>> _mockLogger;



        public UpdateUserCommandHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<UpdateUserCommandHandler>>();



            _handler = new UpdateUserCommandHandler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockCurrentUser.Object,
                _mockMediator.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_NonAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new UpdateUserDto(
                Id: Guid.NewGuid(),
                Identifiant: "nouveauNom",
                Role: UserRoleDto.Enseignant,
                Statut: UserStatusDto.Actif,
                Email: "nouveau@example.com"
            );
            var command = new UpdateUserCommand(dto);

            _mockCurrentUser
                .Setup(x => x.IsInRole(It.IsAny<string>()))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Only admins can update users.");

            _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new UpdateUserDto(
                Id: userId,
                Identifiant: "nouveauNom",
                Role: UserRoleDto.Admin,
                Statut: UserStatusDto.Inactif,
                Email: "nouveau@example.com"
            );
            var command = new UpdateUserCommand(dto);

            _mockCurrentUser
                .Setup(x => x.IsInRole(RoleUtilisateur.Role.Admin.ToString()))
                .Returns(true);

            _mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Utilisateur)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<KeyNotFoundException>()
                     .WithMessage("User not found.");

            _mockRepo.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Admin_UpdatesUser_SavesAndPublishesEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new Utilisateur(
                id: userId,
                identifiant: "ancienNom",

                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("ancien@example.com")
            );
            existingUser.ClearDomainEvents();

            _mockCurrentUser
                .Setup(x => x.IsInRole(RoleUtilisateur.Role.Admin.ToString()))
                .Returns(true);

            _mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var dto = new UpdateUserDto(
                Id: userId,
                Identifiant: "nouveauNom",
                Role: UserRoleDto.Admin,
                Statut: UserStatusDto.Inactif,
                Email: "nouveau@example.com"
            );
            var command = new UpdateUserCommand(dto);

            // SaveChangesAsync renvoie un int
            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var publishedEvents = new List<INotification>();
            _mockMediator
                .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Callback<INotification, CancellationToken>((evt, _) => publishedEvents.Add(evt))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);

            // Vérifier que les propriétés ont été mises à jour
            existingUser.Identifiant.Should().Be("nouveauNom");
            existingUser.Email.Value.Should().Be("nouveau@example.com");
            existingUser.Role.Valeur.Should().Be(RoleUtilisateur.Role.Admin);
            existingUser.Statut.Value.Should().Be(StatutUtilisateur.StatutEnum.Inactif);

            // SaveChangesAsync appelé une fois
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Un événement a été publié et c'est du type UserUpdatedEvent
            publishedEvents.Should().HaveCount(1);
            publishedEvents[0].Should().BeOfType<UserUpdatedEvent>();

        }
    }
}

