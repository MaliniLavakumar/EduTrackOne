using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Matieres;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class MatiereTests
    {
        // Méthode utilitaire pour générer un NomMatiere valide
        private NomMatiere CréerNomMatiere() => new NomMatiere("Mathématiques");

        [Fact]
        public void Constructeur_NomNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            Action act = () => new Matiere(id, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nom");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nom = CréerNomMatiere();

            // Act
            var matiere = new Matiere(id, nom);

            // Assert
            matiere.Id.Should().Be(id);
            matiere.Nom.Should().Be(nom);
            matiere.GetNom().Should().Be("Mathématiques");
        }

        [Fact]
        public void Creer_StaticAvecNomNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            Action act = () => Matiere.Creer(id, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nom");
        }

        [Fact]
        public void Creer_StaticValide_Doit_CréerInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nom = CréerNomMatiere();

            // Act
            var matiere = Matiere.Creer(id, nom);

            // Assert
            matiere.Id.Should().Be(id);
            matiere.Nom.Should().Be(nom);
            matiere.GetNom().Should().Be("Mathématiques");
        }
    }
}
