using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Presences;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class PresenceTests
    {
        // Génère un GUID valide
        private Guid GuidValide() => Guid.NewGuid();

        [Fact]
        public void Constructeur_StatutNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var idInscription = GuidValide();
            var id = GuidValide();
            var date = new DateTime(2025, 05, 10);
            var periode = 1;
            StatutPresence statut = null!;

            // Act
            Action act = () => new Presence(id, date, periode, statut!, idInscription);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("statut");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var idInscription = GuidValide();
            var id = GuidValide();
            var date = new DateTime(2025, 05, 10);
            var periode = 2;
            var statut = new StatutPresence(StatutPresence.StatutEnum.Present);

            // Act
            var presence = new Presence(id, date, periode, statut, idInscription);

            // Assert
            presence.Id.Should().Be(id);
            presence.Date.Should().Be(date);
            presence.Periode.Should().Be(periode);
            presence.Statut.Should().Be(statut);
            presence.IdInscription.Should().Be(idInscription);
        }

        [Fact]
        public void Modifier_Valide_Doit_MettreAJourPropriétés()
        {
            // Arrange
            var idInscription = GuidValide();
            var presence = new Presence(
                GuidValide(),
                new DateTime(2025, 05, 10),
                1,
                new StatutPresence(StatutPresence.StatutEnum.Absent),
                idInscription
            );

            var nouvelleDate = new DateTime(2025, 06, 15);
            var nouvellePeriode = 3;
            var nouveauStatut = new StatutPresence(StatutPresence.StatutEnum.Present);

            // Act
            presence.Modifier(nouvelleDate, nouvellePeriode, nouveauStatut);

            // Assert
            presence.Date.Should().Be(nouvelleDate);
            presence.Periode.Should().Be(nouvellePeriode);
            presence.Statut.Should().Be(nouveauStatut);
        }

        [Fact]
        public void ToString_Doit_RetournerChaineFormatée()
        {
            // Arrange
            var idInscription = GuidValide();
            var date = new DateTime(2025, 07, 20);
            var periode = 2;
            var statut = new StatutPresence(StatutPresence.StatutEnum.Absent);
            var presence = new Presence(GuidValide(), date, periode, statut, idInscription);

            // Act
            var result = presence.ToString();

            // Assert
            result.Should().Be($"Date: {date.ToShortDateString()}, Periode: {periode}, Statut: {statut.Value}");
        }
    }
}
