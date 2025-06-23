using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using EduTrackOne.Domain.Eleves;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class EnseignantPrincipalTests
    {
        // Méthodes utilitaires pour créer des Value Objects valides
        private NomComplet CréerNomComplet() => new NomComplet("Jean", "Dupont");
        private Email CréerEmail() => new Email("jean.dupont@example.com");

        [Fact]
        public void Constructeur_Null_NomComplet_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var email = CréerEmail();

            // Act
            Action act = () => new EnseignantPrincipal(id, null!, email);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nomComplet");
        }

        [Fact]
        public void Constructeur_Null_Email_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();

            // Act
            Action act = () => new EnseignantPrincipal(id, nomComplet, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("email");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var email = CréerEmail();

            // Act
            var enseignant = new EnseignantPrincipal(id, nomComplet, email);

            // Assert
            enseignant.Id.Should().Be(id);
            enseignant.NomComplet.Should().Be(nomComplet);
            enseignant.Email.Should().Be(email);
            enseignant.Classes.Should().BeEmpty();
        }

        [Fact]
        public void ModifierCoordonnees_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var enseignant = new EnseignantPrincipal(id, CréerNomComplet(), CréerEmail());

            // Act
            Action act = () => enseignant.ModifierCoordonnees(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nouvelEmail");
        }

        [Fact]
        public void ModifierCoordonnees_Valide_Doit_MettreAJourEmail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var enseignant = new EnseignantPrincipal(id, CréerNomComplet(), CréerEmail());
            var nouveauEmail = new Email("nouveau.email@example.com");

            // Act
            enseignant.ModifierCoordonnees(nouveauEmail);

            // Assert
            enseignant.Email.Should().Be(nouveauEmail);
        }

        [Fact]
        public void GetFullName_Doit_RetournerPrenomEspaceNom()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = new NomComplet("Marie", "Martin");
            var enseignant = new EnseignantPrincipal(id, nomComplet, CréerEmail());

            // Act
            var result = enseignant.GetFullName();

            // Assert
            result.Should().Be("Marie Martin");
        }
    }
}
