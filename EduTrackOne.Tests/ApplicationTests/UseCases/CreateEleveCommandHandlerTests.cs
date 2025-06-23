using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Threading;     // ← nécessaire pour CancellationToken
using System.Threading.Tasks;
using Xunit;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class CreateEleveCommandHandlerTests
    {
        private readonly Mock<IEleveRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IValidator<CreateEleveCommand>> _mockValidator;
        private readonly CreateEleveCommandHandler _handler;

        public CreateEleveCommandHandlerTests()
        {
            _mockRepo = new Mock<IEleveRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<CreateEleveCommand>>();
            _handler = new CreateEleveCommandHandler(
                                 _mockRepo.Object,
                                 _mockUow.Object,
                                 _mockValidator.Object
                             );
        }

        private CreateEleveCommand CreateValidCommand()
        {
            return new CreateEleveCommand(
                Prenom: "Sophie",
                Nom: "Durand",
                DateNaissance: new DateTime(2010, 5, 12),
                Sexe: "Fille",
                Rue: "Rue de la Paix 10",
                CodePostal: "1000",
                Ville: "Lausanne",
                EmailParent: "parent@example.com",
                Tel1: "+41791234567",
                Tel2: "+41790000000",
                NoImmatricule: "ELV-2025-001"
            );
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsFailureResult()
        {
            // Arrange
            var command = CreateValidCommand();
            var failures = new[]
            {
                new ValidationFailure("Prenom", "Prénom requis"),
                new ValidationFailure("Nom",   "Nom requis")
            };
            var invalidResult = new ValidationResult(failures);

            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error
                  .Should().Contain("Erreurs de validation")
                  .And.Contain("Prénom requis")
                  .And.Contain("Nom requis");

            _mockRepo.Verify(
                r => r.ExistsByNoImmatriculeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never()
            );
            _mockRepo.Verify(
                r => r.AddAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()),
                Times.Never()
            );
            _mockUow.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task Handle_ImmatriculeAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var command = CreateValidCommand();
            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valide

            _mockRepo
                .Setup(r => r.ExistsByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Un élève avec ce numéro d'immatricule existe déjà.");

            _mockRepo.Verify(
                r => r.ExistsByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _mockRepo.Verify(
                r => r.AddAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()),
                Times.Never()
            );
            _mockUow.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsEleveAndReturnsSuccess()
        {
            // Arrange
            var command = CreateValidCommand();
            _mockValidator
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valide

            _mockRepo
                .Setup(r => r.ExistsByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Eleve capturedEleve = null!;
            _mockRepo
                .Setup(r => r.AddAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()))
                .Callback<Eleve, CancellationToken>((e, ct) => capturedEleve = e)
                .Returns(Task.CompletedTask);

            _mockUow
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            capturedEleve.Should().NotBeNull();
            capturedEleve.NomComplet.ToString()
                         .Should().Be($"{command.Prenom} {command.Nom}");
            capturedEleve.DateNaissance.Value
                         .Should().Be(command.DateNaissance);
            capturedEleve.Sexe.Value
                         .Should().Be(Enum.Parse<Sexe.SexeType>(command.Sexe, true));
            capturedEleve.Adresse.ToString()
                         .Should().Be($"{command.Rue}, {command.CodePostal}, {command.Ville}");
            capturedEleve.EmailParent.Value.Should().Be(command.EmailParent);
            capturedEleve.Tel1.Value.Should().Be(command.Tel1);
            capturedEleve.Tel2!.Value.Should().Be(command.Tel2);
            capturedEleve.NoImmatricule.Should().Be(command.NoImmatricule);

            _mockRepo.Verify(
                r => r.ExistsByNoImmatriculeAsync(command.NoImmatricule, It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _mockRepo.Verify(
                r => r.AddAsync(It.IsAny<Eleve>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _mockUow.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once()
            );
        }
    }
}
