using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class AddInscriptionCommandHandlerTests
    {
        private readonly Mock<IClasseRepository> _mockClasseRepo;
        private readonly Mock<IEleveRepository> _mockEleveRepo;
        private readonly Mock<IInscriptionRepository> _mockInscRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IValidator<AddInscriptionCommand>> _mockValidator;
        private readonly Mock<ILogger<AddInscriptionCommandHandler>> _mockLogger;
        private readonly AddInscriptionCommandHandler _handler;

        public AddInscriptionCommandHandlerTests()
        {
            _mockClasseRepo = new Mock<IClasseRepository>();
            _mockEleveRepo = new Mock<IEleveRepository>();
            _mockInscRepo = new Mock<IInscriptionRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<AddInscriptionCommand>>();
            _mockLogger = new Mock<ILogger<AddInscriptionCommandHandler>>();
            _handler = new AddInscriptionCommandHandler(
                _mockClasseRepo.Object,
                _mockEleveRepo.Object,
                _mockInscRepo.Object,
                _mockUow.Object,
                _mockValidator.Object,
                _mockLogger.Object
            );
        }

        private Eleve CréerEleve(Guid id, string noImmatricule)
        {
            return new Eleve(
                id,
                new NomComplet("Sophie", "Durand"),
                new DateNaissance(new DateTime(2010, 5, 12)),
                new Sexe(Sexe.SexeType.Fille),
                new Adresse("Rue de la Paix 10", "1000", "Lausanne"),
                new Email("parent@example.com"),
                new Telephone("+41791234567"),
                new Telephone("+41790000000"),
                noImmatricule
            );
        }

        private Classe CréerClasse(Guid id)
        {
            return new Classe(
                id,
                new NomClasse("4P/01-2024"),
                new AnneeScolaire("2024-2025")
            );
        }

        private AddInscriptionCommand CreateValidCommand(Guid classeId, string noImmatricule)
        {
            return new AddInscriptionCommand(
                ClasseId: classeId,
                NoImmatricule: noImmatricule,
                DateDebut: new DateTime(2025, 1, 1),
                DateFin: new DateTime(2025, 6, 1)
            );
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsFailureResult()
        {
            // Arrange
            var command = CreateValidCommand(Guid.NewGuid(), "ELV-001");
            var failures = new[]
            {
                new ValidationFailure(nameof(AddInscriptionCommand.ClasseId), "ClasseId requis"),
                new ValidationFailure(nameof(AddInscriptionCommand.NoImmatricule), "Immatricule requis")
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
            result.Error.Should().Contain("Immatricule requis");

            _mockClasseRepo.Verify(r => r.GetClasseByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockEleveRepo.Verify(r => r.GetByNoImmatriculeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ClasseNotFound_ReturnsFailureResult()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var command = CreateValidCommand(classeId, "ELV-001");

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
            _mockEleveRepo.Verify(r => r.GetByNoImmatriculeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_EleveNotFound_ReturnsFailureResult()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var classe = CréerClasse(classeId);
            var command = CreateValidCommand(classeId, "ELV-001");

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            _mockEleveRepo
                .Setup(r => r.GetByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Eleve)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Aucun élève avec ce numéro d'immatriculation.");

            _mockClasseRepo.Verify(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()), Times.Once());
            _mockEleveRepo.Verify(r => r.GetByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()), Times.Once());
            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_AlreadyRegistered_ReturnsFailureResult()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var noImmat = "ELV-001";

            var eleve = CréerEleve(eleveId, noImmat);
            var classe = CréerClasse(classeId);
            // Pré-inscrire l'élève
            var periodeEx = new DateInscriptionPeriode(DateTime.UtcNow.Date.AddDays(-10), null);
            classe.InscrireEleve(eleveId, periodeEx);

            var command = CreateValidCommand(classeId, noImmat);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            _mockEleveRepo
                .Setup(r => r.GetByNoImmatriculeAsync(noImmat, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eleve);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Cet élève est déjà inscrit dans la classe.");

            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_InvalidPeriod_ReturnsFailureResult()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleve = CréerEleve(Guid.NewGuid(), "ELV-001");
            var classe = CréerClasse(classeId);

            // Dates invalides : fin avant début
            var command = new AddInscriptionCommand(
                ClasseId: classeId,
                NoImmatricule: eleve.NoImmatricule,
                DateDebut: new DateTime(2025, 6, 1),
                DateFin: new DateTime(2025, 1, 1)
            );

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            _mockEleveRepo
                .Setup(r => r.GetByNoImmatriculeAsync(eleve.NoImmatricule, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eleve);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().StartWith("Période invalide");

            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsInscriptionAndReturnsSuccess()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var noImmat = "ELV-001";

            var eleve = CréerEleve(eleveId, noImmat);
            var classe = CréerClasse(classeId);
            var command = CreateValidCommand(classeId, noImmat);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockClasseRepo
                .Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(classe);

            _mockEleveRepo
                .Setup(r => r.GetByNoImmatriculeAsync(noImmat, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eleve);

            Inscription capturedInsc = null!;
            _mockInscRepo
                .Setup(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()))
                .Callback<Inscription, CancellationToken>((i, ct) => capturedInsc = i)
                .Returns(Task.CompletedTask);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            var nouvelleInsc = classe.Inscriptions.Single(i => i.Id == result.Value);
            nouvelleInsc.IdEleve.Should().Be(eleveId);
            nouvelleInsc.IdClasse.Should().Be(classeId);

            _mockInscRepo.Verify(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()), Times.Once());
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            capturedInsc.Should().NotBeNull();
            capturedInsc.Id.Should().Be(nouvelleInsc.Id);
        }

        [Fact]
        public async Task Handle_AddAsyncThrows_PropagatesException()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var noImmat = "ELV-002";
            var eleve = CréerEleve(eleveId, noImmat);
            var classe = CréerClasse(classeId);
            var command = CreateValidCommand(classeId, noImmat);

            _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _mockClasseRepo.Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>())).ReturnsAsync(classe);
            _mockEleveRepo.Setup(r => r.GetByNoImmatriculeAsync(noImmat, It.IsAny<CancellationToken>())).ReturnsAsync(eleve);

            _mockInscRepo
                .Setup(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Erreur base de données"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_SaveChangesThrows_PropagatesException()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var noImmat = "ELV-003";
            var eleve = CréerEleve(eleveId, noImmat);
            var classe = CréerClasse(classeId);
            var command = CreateValidCommand(classeId, noImmat);

            _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _mockClasseRepo.Setup(r => r.GetClasseByIdAsync(classeId, It.IsAny<CancellationToken>())).ReturnsAsync(classe);
            _mockEleveRepo.Setup(r => r.GetByNoImmatriculeAsync(noImmat, It.IsAny<CancellationToken>())).ReturnsAsync(eleve);
            _mockInscRepo.Setup(r => r.AddAsync(It.IsAny<Inscription>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Échec SaveChanges"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}