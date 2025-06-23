using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Domain.Utilisateurs;
using EduTrackOne.Domain.Utilisateurs.Events;
using EduTrackOne.Contracts.DTOs;
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
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<ChangePasswordCommandHandler>> _mockLogger;
        private readonly IPasswordHasher<Utilisateur> _passwordHasher;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ChangePasswordCommandHandler>>();

            // Utiliser la vraie implémentation PasswordHasher pour hash/vérif
            _passwordHasher = new PasswordHasher<Utilisateur>();

            _handler = new ChangePasswordCommandHandler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockCurrentUser.Object,
                _mockMediator.Object,
                _mockLogger.Object,
                _passwordHasher
            );
        }

        [Fact]
        public async Task Handle_UtilisateurNonAutorise_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: "oldPass",
                NouveauMotDePasse: "newPass"
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant qui n'est ni admin ni propriétaire de l'ID cible
            _mockCurrentUser
                .Setup(x => x.IsInRole(It.IsAny<string>()))
                .Returns(false);
            _mockCurrentUser
                .Setup(x => x.UserId)
                .Returns(Guid.NewGuid()); // différent de userId

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Vous n’êtes pas autorisé à changer ce mot de passe.");
            _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UtilisateurNonTrouve_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: "oldPass",
                NouveauMotDePasse: "newPass"
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant autorisé (même userId)
            _mockCurrentUser
                .Setup(x => x.IsInRole(It.IsAny<string>()))
                .Returns(false);
            _mockCurrentUser
                .Setup(x => x.UserId)
                .Returns(userId);

            // Le repository ne trouve pas d'utilisateur
            _mockRepo
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Utilisateur)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Utilisateur introuvable.");
            _mockRepo.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AncienMotDePasseIncorrect_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: "wrongOldPass",
                NouveauMotDePasse: "newPass"
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant autorisé (même userId)
            _mockCurrentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            _mockCurrentUser.Setup(x => x.UserId).Returns(userId);

            // Créer utilisateur avec un hash pour un autre mot de passe
            var utilisateur = new Utilisateur(
                id: userId,
                identifiant: "testuser",
                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("test@example.com")
            );
            // On hash un mot de passe différent de "wrongOldPass"
            utilisateur.ModifierMotDePasse(_passwordHasher.HashPassword(utilisateur, "correctOldPass"));

            // Clear les éventuels DomainEvents initiaux
            utilisateur.ClearDomainEvents();

            _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(utilisateur);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Ancien mot de passe incorrect.");
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NouveauMotDePasseIdentiqueAncien_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldPass = "samePass";
            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: oldPass,
                NouveauMotDePasse: oldPass // identique
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant autorisé
            _mockCurrentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            _mockCurrentUser.Setup(x => x.UserId).Returns(userId);

            // Créer utilisateur avec hash correspondant à oldPass
            var utilisateur = new Utilisateur(
                id: userId,
                identifiant: "testuser",
                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("test@example.com")
            );
            // Hash de oldPass
            utilisateur.ModifierMotDePasse(_passwordHasher.HashPassword(utilisateur, oldPass));
            utilisateur.ClearDomainEvents();

            _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(utilisateur);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Le nouveau mot de passe doit être différent de l’ancien.");
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Succes_ModifieMotDePasse_AppliqueSaveAndPublishesEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldPassword = "oldPass";
            var newPassword = "newPass";

            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: oldPassword,
                NouveauMotDePasse: newPassword
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant autorisé (même userId)
            _mockCurrentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            _mockCurrentUser.Setup(x => x.UserId).Returns(userId);

            // Créer un utilisateur avec hash du oldPassword
            var utilisateur = new Utilisateur(
                id: userId,
                identifiant: "testuser",
                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("test@example.com")
            );
            // Hash du oldPassword
            utilisateur.ModifierMotDePasse(_passwordHasher.HashPassword(utilisateur, oldPassword));
            // Vider les events initiaux
            utilisateur.ClearDomainEvents();

            _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(utilisateur);

            // Mock SaveChangesAsync
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Capturer les events publiés
            var publishedEvents = new List<INotification>();
            _mockMediator
                .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Callback<INotification, CancellationToken>((evt, _) =>
                {
                    publishedEvents.Add(evt);
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(Unit.Value);

            // Vérifier que SaveChangesAsync a été appelé
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Vérifier qu'un seul PasswordChangedEvent a été généré et publié
            // Après ClearDomainEvents avant, on s'attend à exactement 1 event
            publishedEvents.Should().HaveCount(1);
            publishedEvents[0].Should().BeOfType<PasswordChangedEvent>();

            // Vérifier que le DomainEvents sur l'entité a été vidé après publication
            utilisateur.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ExceptionDuringSave_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var oldPassword = "oldPass";
            var newPassword = "newPass";

            var dto = new ChangePasswordDto(
                UserId: userId,
                AncienMotDePasse: oldPassword,
                NouveauMotDePasse: newPassword
            );
            var command = new ChangePasswordCommand(dto);

            // Simuler un utilisateur courant autorisé
            _mockCurrentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            _mockCurrentUser.Setup(x => x.UserId).Returns(userId);

            // Créer un utilisateur avec hash du oldPassword
            var utilisateur = new Utilisateur(
                id: userId,
                identifiant: "testuser",
                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("test@example.com")
            );
            utilisateur.ModifierMotDePasse(_passwordHasher.HashPassword(utilisateur, oldPassword));
            utilisateur.ClearDomainEvents();

            _mockRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(utilisateur);

            // Simuler exception lors de SaveChangesAsync
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Une erreur est survenue lors du changement de mot de passe.");

            // On s'attend à ce que Publish n'ait pas été appelé à cause de l’exception
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
