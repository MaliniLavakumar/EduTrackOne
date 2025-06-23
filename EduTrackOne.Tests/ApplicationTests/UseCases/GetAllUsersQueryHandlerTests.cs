using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Utilisateurs.GetAllUsers;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly GetAllUsersQueryHandler _handler;
        private readonly Mock<ILogger<GetAllUsersQueryHandler>> _mockLogger;
        public GetAllUsersQueryHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _handler = new GetAllUsersQueryHandler(_mockRepo.Object, _mockLogger.Object);

            _mockLogger = new Mock<ILogger<GetAllUsersQueryHandler>>();

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
            var userId1 = Guid.NewGuid();
            var user1 = new Utilisateur(
                id: userId1,
                identifiant: "alice",
               
                role: new RoleUtilisateur(RoleUtilisateur.Role.Admin),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif),
                email: new Email("alice@example.com")
            );

            var userId2 = Guid.NewGuid();
            var user2 = new Utilisateur(
                id: userId2,
                identifiant: "bob",
               
                role: new RoleUtilisateur(RoleUtilisateur.Role.Enseignant),
                statut: new StatutUtilisateur(StatutUtilisateur.StatutEnum.Inactif),
                email: new Email("bob@example.com")
            );

            // Clear any domain events raised during construction
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

            var dtoList = new List<GetAllUsersDto>(result);
            dtoList[0].Id.Should().Be(userId1);
            dtoList[0].Identifiant.Should().Be("alice");
            dtoList[0].Role.Should().Be(RoleUtilisateur.Role.Admin);
            dtoList[0].Statut.Should().Be(StatutUtilisateur.StatutEnum.Actif);
            dtoList[0].Email.Should().Be("alice@example.com");

            dtoList[1].Id.Should().Be(userId2);
            dtoList[1].Identifiant.Should().Be("bob");
            dtoList[1].Role.Should().Be(RoleUtilisateur.Role.Enseignant);
            dtoList[1].Statut.Should().Be(StatutUtilisateur.StatutEnum.Inactif);
            dtoList[1].Email.Should().Be("bob@example.com");

            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

