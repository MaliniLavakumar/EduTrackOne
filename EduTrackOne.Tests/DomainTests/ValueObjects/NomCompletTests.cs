using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class NomCompletTests
    {
        [Theory]
        [InlineData("Sophie", "Durand")]
        [InlineData("Jean", "Dupond")]
        [InlineData("Marie", "Martin")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string prenom, string nom)
        {
            // Act
            var nomComplet = new NomComplet(prenom, nom);

            // Assert
            nomComplet.Prenom.Should().Be(prenom);
            nomComplet.Nom.Should().Be(nom);
            nomComplet.ToString().Should().Be($"{prenom} {nom}");
        }

        [Theory]
        [InlineData("", "Durand")]        // prénom vide
        [InlineData("Sophie", "")]        // nom vide
        [InlineData("   ", "Durand")]     // prénom uniquement espaces
        [InlineData("Sophie", "   ")]     // nom uniquement espaces
        [InlineData(null, "Durand")]      // prénom null
        [InlineData("Sophie", null)]      // nom null
        public void Constructeur_PrenomOuNomInvalide_Doit_LeverArgumentException(string prenom, string nom)
        {
            // Act
            Action act = () => new NomComplet(prenom, nom);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Le prénom et le nom doivent être valides.");
        }

        [Fact]
        public void ToString_AppelleSurNomCompletSansEspacesAdditionnels_RetournePrenomEspaceNom()
        {
            // Arrange
            var prenom = "  Sophie  ";
            var nom = "  Durand  ";
            var nomComplet = new NomComplet(prenom.Trim(), nom.Trim());

            // Act
            var result = nomComplet.ToString();

            // Assert
            result.Should().Be("Sophie Durand");
        }

    }
}
