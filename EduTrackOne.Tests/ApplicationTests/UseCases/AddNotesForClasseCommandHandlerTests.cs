using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Application.Notes.GetNotesByInscription;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
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
    public class AddNotesForClasseCommandHandlerTests
    {
        private readonly Mock<IInscriptionRepository> _inscRepoMock = new();
        private readonly Mock<INoteRepository> _noteRepoMock = new();
        private readonly Mock<IInscriptionManager> _managerMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<ILogger<AddNotesForClasseCommandHandler>> _mockLogger;
        private readonly Mock<IValidator<AddNotesForClasseCommand>> _validatorMock = new();
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
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var matiereId = Guid.NewGuid();
            var dateExamen = new DateTime(2025, 5, 10);

            var dateDebut = new DateOnly(2024, 9, 1);
            var dateFin = new DateOnly(2025, 6, 30);
            var periode = new DateInscriptionPeriode(
                dateDebut.ToDateTime(TimeOnly.MinValue),
                dateFin.ToDateTime(TimeOnly.MinValue)
                );
            
                        var noteDto = new NoteForEleveDto(eleveId, matiereId, 5.0, "Bon travail");

            var command = new AddNotesForClasseCommand(
                classeId,
                dateExamen,
                new List<NoteForEleveDto> { noteDto }
            );

            var inscription = new Inscription(inscriptionId, periode, classeId, eleveId);
            var inscriptions = new List<Inscription> { inscription };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<AddNotesForClasseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _inscRepoMock
                .Setup(r => r.GetByClasseAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscriptions);

            _noteRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

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

            _noteRepoMock.Verify(r => r.AddAsync(It.IsAny<Note>(), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}