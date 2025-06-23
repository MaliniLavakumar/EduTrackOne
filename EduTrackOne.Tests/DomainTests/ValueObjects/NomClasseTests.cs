using EduTrackOne.Domain.Classes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class NomClasseTests
    {
        [Theory]
        [InlineData("4P/01-2024")]
        [InlineData("10B/12-1999")]
        [InlineData("1A/05-2021")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string valeur)
        {
            // Act
            var nomClasse = new NomClasse(valeur);

            // Assert
            nomClasse.Value.Should().Be(valeur);
            nomClasse.ToString().Should().Be(valeur);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructeur_VideOuNull_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new NomClasse(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Le nom de la classe ne peut pas être vide.");
        }

        [Theory]
        [InlineData("4P01-2024")]      // manque le slash
        [InlineData("4P/012024")]      // manque le tiret
        [InlineData("P4/01-2024")]     // chiffre mal placé
        [InlineData("4P/1-2024")]      // mois sur un chiffre
        [InlineData("4P/01-24")]       // année sur deux chiffres
        [InlineData("ClasseX")]        // format complètement différent
        public void Constructeur_FormatIncorrect_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new NomClasse(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Le nom de la classe doit suivre le format attendu (ex: 4P/01-2024).");
        }

    }
}
