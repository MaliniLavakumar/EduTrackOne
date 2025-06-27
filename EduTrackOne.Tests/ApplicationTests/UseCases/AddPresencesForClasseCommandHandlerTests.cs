using Xunit;                                          // +++
using EduTrackOne.Application.Inscriptions.AddPresencesForClasse;
using EduTrackOne.Contracts.DTOs;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Presences;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class AddPresencesForClasseCommandHandlerTests
    {
        private readonly Mock<IInscriptionRepository> _inscRepoMock = new();
        private readonly Mock<IPresenceRepository> _presRepoMock = new();
        private readonly Mock<IInscriptionManager> _managerMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IValidator<AddPresencesForClasseCommand>> _validatorMock = new();
        private readonly Mock<ILogger<AddPresencesForClasseCommandHandler>> _mockLogger
            = new Mock<ILogger<AddPresencesForClasseCommandHandler>>();  // +++

        private readonly AddPresencesForClasseCommandHandler _handler;

        public AddPresencesForClasseCommandHandlerTests()
        {
            _handler = new AddPresencesForClasseCommandHandler(
                _inscRepoMock.Object,
                _presRepoMock.Object,
                _managerMock.Object,
                _uowMock.Object,
                _validatorMock.Object,
                _mockLogger.Object    // +++
            );
        }

        [Fact]
        public async Task Handle_ShouldAddPresencesAndReturnSuccessCount()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var inscriptionId = Guid.NewGuid();
            var date = new DateTime(2025, 6, 5);
            var periode = 1;

            var presenceDto = new PresenceEleveDto(eleveId, "Present");
            var command = new AddPresencesForClasseCommand(
                classeId,
                date,
                periode,
                new List<PresenceEleveDto> { presenceDto }
            );

            var periodeInscription = new DateInscriptionPeriode(new DateTime(2025, 1, 1), null);
            var inscription = new Inscription(inscriptionId, periodeInscription, classeId, eleveId);
            var inscriptions = new List<Inscription> { inscription };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<AddPresencesForClasseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _inscRepoMock
                .Setup(r => r.GetByClasseAsync(classeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscriptions);

            _presRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Presence>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uowMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);  // +++

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(1);

            _managerMock.Verify(m => m.MarquerPresence(
                It.Is<Inscription>(i => i.Id == inscriptionId),
                It.Is<Presence>(p =>
                    p.IdInscription == inscriptionId &&
                    p.Date == date &&
                    p.Periode == periode &&
                    p.Statut.Value == StatutPresence.StatutEnum.Present
                )
            ), Times.Once);

            _presRepoMock.Verify(r => r.AddAsync(
                It.IsAny<Presence>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
