using Xunit;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class AddNotesForClasseCommandHandlerTests
    {
        private readonly Mock<IInscriptionRepository> _inscRepoMock = new();
        private readonly Mock<INoteRepository> _noteRepoMock = new();
        private readonly Mock<IInscriptionManager> _managerMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IValidator<AddNotesForClasseCommand>> _validatorMock = new();
        private readonly Mock<ILogger<AddNotesForClasseCommandHandler>> _mockLogger
            = new Mock<ILogger<AddNotesForClasseCommandHandler>>();
        private readonly AddNotesForClasseCommandHandler _handler;

        public AddNotesForClasseCommandHandlerTests()
        {
            _handler = new AddNotesForClasseCommandHandler(
                _inscRepoMock.Object,
                _noteRepoMock.Object,
                _managerMock.Object,
                _uowMock.Object,
                _validatorMock.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldAddNotesAndReturnSuccessCount()
        {
            // Arrange
            var inscriptionId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var matiereId = Guid.NewGuid();
            var dateExamen = new DateTime(2025, 5, 10);

            var periode = new DateInscriptionPeriode(
                new DateTime(2024, 9, 1),
                new DateTime(2025, 6, 30)
            );

            var noteDto = new NoteForEleveDto(eleveId, matiereId, 5.0, "Bon travail");
            var command = new AddNotesForClasseCommand(
                Guid.NewGuid(), // classeId
                dateExamen,
                new List<NoteForEleveDto> { noteDto }
            );

            var inscription = new Inscription(inscriptionId, periode, command.ClasseId, eleveId);
            _inscRepoMock
    .Setup(r => r.GetByClasseAsync(command.ClasseId, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new List<Inscription> { inscription });


            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _noteRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(1);

            _managerMock.Verify(m => m.AjouterNote(
                It.Is<Inscription>(i => i.Id == inscriptionId),
                It.Is<Note>(n =>
                    n.IdInscription == inscriptionId &&
                    n.IdMatiere == matiereId &&
                    n.Valeur.Value == 5.0 &&
                    n.Commentaire != null &&
                    n.Commentaire.Value == "Bon travail"
                )
            ), Times.Once);

            _noteRepoMock.Verify(r => r.AddAsync(
                It.IsAny<Note>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
