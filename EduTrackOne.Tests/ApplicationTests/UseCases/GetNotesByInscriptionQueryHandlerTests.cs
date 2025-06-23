using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Notes.GetNotesByInscription;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using EduTrackOne.Domain.Notes;
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
    public class GetNotesByInscriptionQueryHandlerTests
    {
        private readonly Mock<IInscriptionRepository> _mockInscriptionRepo;
        private readonly Mock<IMatiereRepository> _mockMatiereRepo;
        private readonly GetNotesByInscriptionQueryHandler _handler;
        private readonly Mock<ILogger<GetNotesByInscriptionQueryHandler>> _mockLogger;


        public GetNotesByInscriptionQueryHandlerTests()
        {
            _mockInscriptionRepo = new Mock<IInscriptionRepository>();
            _mockMatiereRepo = new Mock<IMatiereRepository>();
            _mockLogger = new Mock<ILogger<GetNotesByInscriptionQueryHandler>>();


            _handler = new GetNotesByInscriptionQueryHandler(_mockInscriptionRepo.Object, _mockMatiereRepo.Object, _mockLogger.Object
);
        }

        [Fact]
        public async Task Handle_WithValidInscriptionId_ReturnsPaginatedNotes()
        {
            // Arrange
            var inscriptionId = Guid.NewGuid();
            var matiereId = Guid.NewGuid();
            var dateExamen = new DateTime(2025, 6, 1);
            var dateDebut = new DateOnly(2024, 9, 1);
            var dateFin = new DateOnly(2025, 6, 30);
            var periode = new DateInscriptionPeriode(
                dateDebut.ToDateTime(TimeOnly.MinValue),
                dateFin.ToDateTime(TimeOnly.MinValue)
                );

            var note1 = new Note(Guid.NewGuid(),dateExamen, new ValeurNote(5.0), null, inscriptionId, matiereId);
            var note2 = new Note(Guid.NewGuid(), dateExamen.AddDays(1), new ValeurNote(4.0), null, inscriptionId, matiereId); 

            var notes = new List<Note> { note1, note2 };
            var inscription = new Inscription(inscriptionId,periode, Guid.NewGuid(), Guid.NewGuid());
            inscription.AjouterNote(note1);
            inscription.AjouterNote(note2);

            var matiere = new Matiere(matiereId, new NomMatiere("Mathématiques"));

            _mockInscriptionRepo
                .Setup(r => r.GetByIdAsync(inscriptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscription);

            _mockMatiereRepo
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Matiere> { matiere });

            var query = new GetNotesByInscriptionQuery(
                inscriptionId:inscriptionId,
                matiereId: null,
                dateDebut: null,
                dateFin: null,
                pageNumber: 1,
                pageSize: 10
            );

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalItems.Should().Be(1); // une matière
            result.Items.Should().HaveCount(1);

            var dto = result.Items.First();
            dto.MatiereId.Should().Be(matiereId);
            dto.MatiereNom.Should().Be("Mathématiques");
            dto.Notes.Should().HaveCount(2);
            dto.Moyenne.Should().Be(4.5); // (5 + 4) / 2
        }

        [Fact]
        public async Task Handle_WithInvalidInscriptionId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            _mockInscriptionRepo
                .Setup(r => r.GetByIdAsync(invalidId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Inscription)null!);

            var query = new GetNotesByInscriptionQuery(
                inscriptionId: invalidId,
                matiereId: null,
                dateDebut: null,
                dateFin: null,
                pageNumber: 1,
                pageSize: 10
            );

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Inscription introuvable.");
        }
    }

}

