using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes.GetClasseById;
using EduTrackOne.Domain.Classes;
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
    public class GetClasseByIdHandlerTests
    {
        private readonly Mock<IClasseRepository> _mockRepo;
        private readonly GetClasseByIdHandler _handler;
        private readonly Mock<ILogger<GetClasseByIdHandler>> _mockLogger;
        public GetClasseByIdHandlerTests()
        {
            _mockRepo = new Mock<IClasseRepository>();
            _mockLogger = new Mock<ILogger<GetClasseByIdHandler>>();

            _handler = new GetClasseByIdHandler(_mockRepo.Object, _mockLogger.Object
);
           
        }

        private Classe CréerClasse(Guid id, Guid? idEnseignantPrincipal = null)
        {
            var nom = new NomClasse("4P/01-2024");
            var annee = new AnneeScolaire("2024-2025");
            var classe = new Classe(id, nom, annee);
            if (idEnseignantPrincipal.HasValue)
            {
                classe.AssignerEnseignantPrincipal(idEnseignantPrincipal.Value);
            }
            return classe;
        }

        [Fact]
        public async Task Handle_ClasseNotFound_ReturnsFailureResult()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            _mockRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Classe)null!);

            var query = new GetClasseByIdQuery(classeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Classe introuvable.");
            result.Value.Should().BeNull();

            _mockRepo.Verify(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Handle_ClasseFound_ReturnsSuccessWithDto()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var enseignantId = Guid.NewGuid();
            var classe = CréerClasse(classeId, enseignantId);

            _mockRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            var query = new GetClasseByIdQuery(classeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeNull();

            var dto = result.Value;
            dto.Should().NotBeNull();
            dto.Id.Should().Be(classeId);
            dto.NomClasse.Should().Be("4P/01-2024");
            dto.AnneeScolaire.Should().Be("2024-2025");
            dto.IdEnseignantPrincipal.Should().Be(enseignantId);

            _mockRepo.Verify(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()), Times.Once());
        }

    }
}
