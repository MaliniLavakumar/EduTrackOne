using EduTrackOne.Application.Eleves.GetEleveById;
using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class GetEleveByIdHandlerTests
    {
        private readonly Mock<IEleveRepository> _mockRepo;
        private readonly GetEleveByIdHandler _handler;

        public GetEleveByIdHandlerTests()
        {
            _mockRepo = new Mock<IEleveRepository>();
            _handler = new GetEleveByIdHandler(_mockRepo.Object);
        }

        private Eleve CréerEleve(Guid id)
        {
            var nomComplet = new NomComplet("Sophie", "Durand");
            var dateNaiss = new DateNaissance(new DateTime(2010, 5, 12));
            var sexe = new Sexe(Sexe.SexeType.Fille);
            var adresse = new Adresse("Rue de la Paix 10", "1000", "Lausanne");
            var emailParent = new Email("parent@example.com");
            var tel1 = new Telephone("+41791234567");
            var tel2 = new Telephone("+41790000000");
            var noImmat = "ELV12345";

            return new Eleve(
                id,
                nomComplet,
                dateNaiss,
                sexe,
                adresse,
                emailParent,
                tel1,
                tel2,
                noImmat
            );
        }

        [Fact]
        public async Task Handle_EleveNotFound_ReturnsFailureResult()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _mockRepo
                .Setup(r => r.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Eleve)null!);

            var query = new GetEleveByIdQuery(requestId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Élève non trouvé.");
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task Handle_EleveExists_ReturnsSuccessWithDto()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var eleveEntity = CréerEleve(requestId);

            _mockRepo
                .Setup(r => r.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eleveEntity);

            var query = new GetEleveByIdQuery(requestId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();

            var dto = result.Value;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(requestId);
            dto.Prenom.Should().Be("Sophie");
            dto.Nom.Should().Be("Durand");
            dto.DateNaissance.Should().Be(new DateTime(2010, 5, 12));
            dto.Sexe.Should().Be("Fille");
            dto.Adresse.Should().Be("Rue de la Paix 10, 1000, Lausanne");
            dto.EmailParent.Should().Be("parent@example.com");
            dto.Tel1.Should().Be("+41791234567");
            dto.Tel2.Should().Be("+41790000000");
            dto.NoImmatricule.Should().Be("ELV12345");
        }
    }
}

