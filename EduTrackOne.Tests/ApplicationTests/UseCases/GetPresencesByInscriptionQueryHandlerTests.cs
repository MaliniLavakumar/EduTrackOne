using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Presences;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
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
    public class GetPresencesByInscriptionQueryHandlerTests
    {
        private readonly Mock<IInscriptionRepository> _mockInscriptionRepo;
        private readonly GetPresencesByInscriptionQueryHandler _handler;
        private readonly Mock<ILogger<GetPresencesByInscriptionQueryHandler>> _mockLogger;

        public GetPresencesByInscriptionQueryHandlerTests()
        {
            _mockInscriptionRepo = new Mock<IInscriptionRepository>();
            _mockLogger = new Mock<ILogger<GetPresencesByInscriptionQueryHandler>>();

            _handler = new GetPresencesByInscriptionQueryHandler(_mockInscriptionRepo.Object, _mockLogger.Object
);
        }

        [Fact]
        public async Task Handle_WithValidInscriptionId_ReturnsCorrectAbsencesAndPresences()
        {
            // Arrange
            var inscriptionId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var classeId = Guid.NewGuid();
            var periode = new DateInscriptionPeriode(DateTime.UtcNow.AddMonths(-1), null);

            var presence1 = new Presence(Guid.NewGuid(), new DateTime(2025, 6, 3), 1, new StatutPresence(StatutPresence.StatutEnum.Absent), inscriptionId);
            var presence2 = new Presence(Guid.NewGuid(), new DateTime(2025, 6, 3), 2, new StatutPresence(StatutPresence.StatutEnum.Present), inscriptionId);
            var presence3 = new Presence(Guid.NewGuid(), new DateTime(2025, 6, 4), 1, new StatutPresence(StatutPresence.StatutEnum.Absent), inscriptionId);

            var inscription = new Inscription(inscriptionId, periode,eleveId, classeId);
            inscription.MarquerPresence(presence1);
            inscription.MarquerPresence(presence2);
            inscription.MarquerPresence(presence3);

            _mockInscriptionRepo
                .Setup(repo => repo.GetByIdAsync(inscriptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscription);

            var query = new GetPresencesByInscriptionQuery(inscriptionId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.PeriodesPresentes.Should().Be(1);
            result.Absences.Should().HaveCount(2);

            result.Absences.Should().Contain(a => a.Date == new DateTime(2025, 6, 3) && a.Periode == 1);
            result.Absences.Should().Contain(a => a.Date == new DateTime(2025, 6, 4) && a.Periode == 1);
        }

        [Fact]
        public async Task Handle_WithInvalidInscriptionId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            _mockInscriptionRepo
                .Setup(repo => repo.GetByIdAsync(invalidId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Inscription)null!);

            var query = new GetPresencesByInscriptionQuery(invalidId);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Inscription non trouvée.");
        }
    }
}

    