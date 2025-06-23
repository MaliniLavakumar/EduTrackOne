using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class AdresseTests
    {
        [Theory]
        [InlineData("Rue de la Paix 10", "1000", "Lausanne")]
        [InlineData("Avenue de la Gare 5", "1200", "Genève")]
        [InlineData("Chemin du Jorat 12B", "1018", "Rossinière")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string rue, string codePostal, string ville)
        {
            // Act
            var adresse = new Adresse(rue, codePostal, ville);

            // Assert
            adresse.Rue.Should().Be(rue);
            adresse.CodePostal.Should().Be(codePostal);
            adresse.Ville.Should().Be(ville);
            adresse.ToString().Should().Be($"{rue}, {codePostal}, {ville}");
        }

        [Theory]
        [InlineData(null, "1000", "Lausanne")]     // rue null
        [InlineData("", "1000", "Lausanne")]       // rue vide
        [InlineData("   ", "1000", "Lausanne")]    // rue uniquement espaces
        [InlineData("Rue Exemple", null, "Lausanne")]  // codePostal null
        [InlineData("Rue Exemple", "", "Lausanne")]    // codePostal vide
        [InlineData("Rue Exemple", "   ", "Lausanne")] // codePostal espaces
        [InlineData("Rue Exemple", "1000", null)]      // ville null
        [InlineData("Rue Exemple", "1000", "")]        // ville vide
        [InlineData("Rue Exemple", "1000", "   ")]     // ville espaces
        public void Constructeur_ParametreInvalide_Doit_LeverArgumentException(string rue, string codePostal, string ville)
        {
            // Act
            Action act = () => new Adresse(rue, codePostal, ville);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'adresse doit être complète et valide.");
        }
    }
}
