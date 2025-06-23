using EduTrackOne.Application.EnseignantsPrincipaux.GetEnseignantPrincipalByEmail;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class GetEnseignantPrincipalByEmailQueryHandlerTests
    {
        private readonly Mock<IEnseignantPrincipalRepository> _mockRepo;
        private readonly GetEnseignantPrincipalByEmailQueryHandler _handler;

        public GetEnseignantPrincipalByEmailQueryHandlerTests()
        {
            _mockRepo = new Mock<IEnseignantPrincipalRepository>();
            _handler = new GetEnseignantPrincipalByEmailQueryHandler(_mockRepo.Object);
        }

        private EnseignantPrincipal CréerEnseignant(Guid id, string prenom, string nom, string email)
        {
            return new EnseignantPrincipal(
                id,
                new NomComplet(prenom, nom),
                new Email(email)
            );
        }

        [Fact]
        public async Task Handle_NoEnseignantFound_ReturnsFailureResult()
        {
            // Arrange
            var email = "notfound@example.com";
            _mockRepo
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EnseignantPrincipal>());

            var query = new GetEnseignantPrincipalByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Enseignant non trouvé.");
            result.Value.Should().BeNull();

            _mockRepo.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Handle_EnseignantFound_ReturnsSuccessWithDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var prenom = "Marie";
            var nom = "Dupont";
            var email = "marie.dupont@example.com";
            var enseignant = CréerEnseignant(id, prenom, nom, email);

            _mockRepo
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EnseignantPrincipal> { enseignant });

            var query = new GetEnseignantPrincipalByEmailQuery(email);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();

            var dto = result.Value;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(id);
            dto.Prenom.Should().Be(prenom);
            dto.Nom.Should().Be(nom);
            dto.Email.Should().Be(email);

            _mockRepo.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}

