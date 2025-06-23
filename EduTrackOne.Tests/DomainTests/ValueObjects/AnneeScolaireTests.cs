using EduTrackOne.Domain.Classes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class AnneeScolaireTests
    {
        [Theory]
        [InlineData("2024-2025")]
        [InlineData("1999-2000")]
        [InlineData("2020-2021")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string valeur)
        {
            // Act
            var annee = new AnneeScolaire(valeur);

            // Assert
            annee.Value.Should().Be(valeur);
            annee.ToString().Should().Be(valeur);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructeur_VideOuNull_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new AnneeScolaire(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'année scolaire ne peut pas être vide.");
        }

        [Theory]
        [InlineData("20242025")]
        [InlineData("2024/2025")]
        [InlineData("20-2025")]
        [InlineData("2024-25")]
        public void Constructeur_FormatIncorrect_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new AnneeScolaire(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'année scolaire doit suivre le format attendu (ex: 2024-2025).");
        }

        [Theory]
        [InlineData("2024-2026")]
        [InlineData("2025-2024")]
        [InlineData("2024-2024")]
        public void Constructeur_AnneesNonConsecutives_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new AnneeScolaire(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Les années scolaires doivent être consécutives (ex: 2024-2025).");
        }

        [Fact]
        public void Constructeur_PremiereAnneeTropGrande_Doit_LeverArgumentException()
        {
            // Ce cas teste la condition “firstYear >= secondYear” de manière explicite
            string valeur = "2025-2025";

            // Act
            Action act = () => new AnneeScolaire(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Les années scolaires doivent être consécutives (ex: 2024-2025).");
        }

    }
}
