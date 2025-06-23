using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Eleves.UpdateEleve;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class UpdateEleveCommandHandlerTests
    {
        private readonly Mock<IEleveRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IValidator<UpdateEleveCommand>> _mockValidator;
        private readonly UpdateEleveCommandHandler _handler;
        private readonly Mock<ILogger<UpdateEleveCommandHandler>> _mockLogger;
        public UpdateEleveCommandHandlerTests()
        {
            _mockRepo = new Mock<IEleveRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<UpdateEleveCommand>>();
            _mockLogger = new Mock<ILogger<UpdateEleveCommandHandler>>();
            _handler = new UpdateEleveCommandHandler(
                                 _mockRepo.Object,
                                 _mockUow.Object,
                                 _mockValidator.Object,
                                 _mockLogger.Object

                             );
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
            var noImmat = "ELV-2025-001";

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

        private UpdateEleveCommand CreateValidCommand(Guid eleveId)
        {
            return new UpdateEleveCommand(
                EleveId: eleveId,
                Rue: "Nouvelle Rue 5",
                CodePostal: "1200",
                Ville: "Genève",
                Tel1: "+41790000001",
                Tel2: "+41790000002",
                EmailParent: "nouveauparent@example.com"
            );
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsFailureResult()
        {
            // Arrange
            var eleveId = Guid.NewGuid();
            var command = CreateValidCommand(eleveId);

            var failures = new[]
            {
                new ValidationFailure(nameof(UpdateEleveCommand.Rue), "Rue invalide"),
                new ValidationFailure(nameof(UpdateEleveCommand.Ville), "Ville requise")
            };
            var invalidResult = new ValidationResult(failures);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Erreurs de validation");
            result.Error.Should().Contain("Rue invalide");
            result.Error.Should().Contain("Ville requise");

            _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_EleveNotFound_ReturnsFailureResult()
        {
            // Arrange
            var eleveId = Guid.NewGuid();
            var command = CreateValidCommand(eleveId);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valid

            _mockRepo
                .Setup(r => r.GetByIdAsync(eleveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Eleve)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Élève introuvable.");

            _mockRepo.Verify(r => r.GetByIdAsync(eleveId, It.IsAny<CancellationToken>()), Times.Once());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesEleveAndReturnsSuccess()
        {
            // Arrange
            var eleveId = Guid.NewGuid();
            var existing = CréerEleve(eleveId);
            var command = CreateValidCommand(eleveId);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valid

            _mockRepo
                .Setup(r => r.GetByIdAsync(eleveId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(eleveId);

            // Vérifier que les propriétés ont été mises à jour
            existing.Adresse.ToString().Should().Be("Nouvelle Rue 5, 1200, Genève");
            existing.Tel1.Value.Should().Be("+41790000001");
            existing.Tel2!.Value.Should().Be("+41790000002");
            existing.EmailParent.Value.Should().Be("nouveauparent@example.com");

            _mockRepo.Verify(r => r.GetByIdAsync(eleveId, It.IsAny<CancellationToken>()), Times.Once());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
