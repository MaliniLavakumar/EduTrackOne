using EduTrackOne.Application.Utilisateurs.GetAllUsers;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly Mock<ILogger<GetAllUsersQueryHandler>> _mockLogger;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _mockLogger = new Mock<ILogger<GetAllUsersQueryHandler>>();
            _handler = new GetAllUsersQueryHandler(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenNoUsers_ReturnsEmptyEnumerable()
        {
            // Arrange
            _mockRepo
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Utilisateur>());

            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUsersExist_ReturnsMappedDtos()
        {
            // Arrange
            var user1 = new Utilisateur(
                Guid.NewGuid(),
                "alice",
                new RoleUtilisateur(RoleUtilisateur.Role.Admin),
                new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                new Email("alice@example.com")
            );
            var user2 = new Utilisateur(
                Guid.NewGuid(),
                "bob",
                new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                new StatutUtilisateur(StatutUtilisateur.StatutEnum.Inactif),
                new Email("bob@example.com")
            );
            user1.ClearDomainEvents();
            user2.ClearDomainEvents();

            _mockRepo
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Utilisateur> { user1, user2 });

            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);

            var dtos = result.ToList();
            dtos[0].Id.Should().Be(user1.Id);
            dtos[0].Identifiant.Should().Be("alice");
            dtos[0].Role.Should().Be(RoleUtilisateur.Role.Admin);
            dtos[0].Statut.Should().Be(StatutUtilisateur.StatutEnum.Actif);
            dtos[0].Email.Should().Be("alice@example.com");

            dtos[1].Id.Should().Be(user2.Id);
            dtos[1].Identifiant.Should().Be("bob");
            dtos[1].Role.Should().Be(RoleUtilisateur.Role.Enseignant);
            dtos[1].Statut.Should().Be(StatutUtilisateur.StatutEnum.Inactif);
            dtos[1].Email.Should().Be("bob@example.com");

            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
