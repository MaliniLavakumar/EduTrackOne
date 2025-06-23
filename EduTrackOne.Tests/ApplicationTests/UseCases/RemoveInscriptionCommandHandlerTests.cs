using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Inscriptions;
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
    public class RemoveInscriptionCommandHandlerTests
    {
        private readonly Mock<IClasseRepository> _mockClasseRepo;
        private readonly Mock<IInscriptionRepository> _mockInscRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IValidator<RemoveInscriptionCommand>> _mockValidator;
        private readonly RemoveInscriptionCommandHandler _handler;
        private readonly Mock<ILogger<RemoveInscriptionCommandHandler>> _mockLogger;

        public RemoveInscriptionCommandHandlerTests()
        {
            _mockClasseRepo = new Mock<IClasseRepository>();
            _mockInscRepo = new Mock<IInscriptionRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<RemoveInscriptionCommand>>();
            _mockLogger = new Mock<ILogger<RemoveInscriptionCommandHandler>>();

            _handler = new RemoveInscriptionCommandHandler(
                _mockClasseRepo.Object,
                _mockUow.Object,
                _mockValidator.Object,
                _mockInscRepo.Object,
                _mockLogger.Object
            );
        }

        private Classe CréerClasseAvecInscription(Guid classeId, Guid eleveId, out Inscription inscription)
        {
            var classe = new Classe(
                classeId,
                new NomClasse("4P/01-2024"),
                new AnneeScolaire("2024-2025")
            );

            // Inscrire un élève pour créer une inscription
            var periode = new DateInscriptionPeriode(DateTime.UtcNow.Date.AddDays(-10), null);
            var inscId = classe.InscrireEleve(eleveId, periode);
            inscription = classe.Inscriptions.Single(i => i.Id == inscId);
            return classe;
        }

        private RemoveInscriptionCommand CreateValidCommand(Guid classeId, Guid eleveId)
        {
            return new RemoveInscriptionCommand(
                IdClasse: classeId,
                IdEleve: eleveId
            );
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsFailure()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var command = CreateValidCommand(classeId, eleveId);

            var failures = new[]
            {
                new ValidationFailure(nameof(RemoveInscriptionCommand.IdClasse), "ClasseId requis"),
                new ValidationFailure(nameof(RemoveInscriptionCommand.IdEleve),  "EleveId requis")
            };
            var invalidResult = new ValidationResult(failures);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("ClasseId requis");
            result.Error.Should().Contain("EleveId requis");

            _mockClasseRepo.Verify(r => r.GetClasseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockInscRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ClasseNotFound_ReturnsFailure()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var command = CreateValidCommand(classeId, eleveId);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Classe)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Classe introuvable.");

            _mockClasseRepo.Verify(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()), Times.Once());
            _mockInscRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_InscriptionNotFound_ReturnsFailure()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var classe = new Classe(
                             classeId,
                             new NomClasse("4P/01-2024"),
                             new AnneeScolaire("2024-2025")
                           );

            var command = CreateValidCommand(classeId, eleveId);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Inscription introuvable.");

            _mockInscRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ValidCommand_RemovesInscriptionAndReturnsSuccess()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            Inscription existingInsc;
            var classe = CréerClasseAvecInscription(classeId, eleveId, out existingInsc);

            var command = CreateValidCommand(classeId, eleveId);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            Inscription? capturedToDelete = null;
            _mockInscRepo
                .Setup(r => r.DeleteAsync(existingInsc.Id, It.IsAny<CancellationToken>()))
                .Callback<Guid, CancellationToken>((id, ct) => capturedToDelete = existingInsc)
                .Returns(Task.CompletedTask);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(existingInsc.Id);

            // La classe ne doit plus contenir cette inscription
            classe.Inscriptions.Any(i => i.Id == existingInsc.Id).Should().BeFalse();

            // Vérifie que DeleteAsync et SaveChangesAsync sont appelés
            _mockInscRepo.Verify(r => r.DeleteAsync(existingInsc.Id, It.IsAny<CancellationToken>()), Times.Once());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            // L’objet capturé dans DeleteAsync est celui qui a été supprimé
            capturedToDelete.Should().NotBeNull();
            capturedToDelete.Id.Should().Be(existingInsc.Id);
        }
    }
}
