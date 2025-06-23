using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Presences;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class StatutPresenceTests
    {
        [Theory]
        [InlineData(StatutPresence.StatutEnum.Present)]
        [InlineData(StatutPresence.StatutEnum.Absent)]
        public void Constructeur_Valide_Doit_CréerUneInstance(StatutPresence.StatutEnum valeur)
        {
            // Act
            var statut = new StatutPresence(valeur);

            // Assert
            statut.Value.Should().Be(valeur);
            statut.ToString().Should().Be(valeur.ToString());
        }

        [Theory]
        [InlineData("Présent", StatutPresence.StatutEnum.Present)]
        [InlineData("present", StatutPresence.StatutEnum.Present)]
        [InlineData("present ", StatutPresence.StatutEnum.Present)]
        [InlineData("  Présent  ", StatutPresence.StatutEnum.Present)]
        [InlineData("Absent", StatutPresence.StatutEnum.Absent)]
        [InlineData("absent", StatutPresence.StatutEnum.Absent)]
        public void FromString_Valide_Doit_RetournerBonEnum(string input, StatutPresence.StatutEnum attendu)
        {
            // Act
            var statut = StatutPresence.FromString(input);

            // Assert
            statut.Value.Should().Be(attendu);
            statut.ToString().Should().Be(attendu.ToString());
            statut.Afficher().Should().Be(attendu == StatutPresence.StatutEnum.Present ? "Présent" : "Absent");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("inconnu")]
        [InlineData("presentee")]
        [InlineData("absente ")]
        public void FromString_Invalide_Doit_LeverArgumentException(string input)
        {
            // Act
            Action act = () => StatutPresence.FromString(input);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Statut invalide. Doit être 'Présent' ou 'Absent'.");
        }
    }
}
