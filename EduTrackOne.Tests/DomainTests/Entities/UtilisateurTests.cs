using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Utilisateurs;
using EduTrackOne.Domain.Eleves;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class UtilisateurTests
    {
        // Méthode utilitaire pour générer un GUID valide
        private Guid GuidValide() => Guid.NewGuid();

        // Méthode utilitaire pour créer un Email valide
        private Email CréerEmail() => new Email("user@example.com");

        // Méthode utilitaire pour créer un RoleUtilisateur valide
        private RoleUtilisateur CréerRoleAdmin() => new RoleUtilisateur(RoleUtilisateur.Role.Admin);
        private RoleUtilisateur CréerRoleEnseignant() => new RoleUtilisateur(RoleUtilisateur.Role.Enseignant);

        // Méthode utilitaire pour créer un StatutUtilisateur valide
        private StatutUtilisateur CréerStatutActif() => new StatutUtilisateur(StatutUtilisateur.StatutEnum.Actif);
        private StatutUtilisateur CréerStatutInactif() => new StatutUtilisateur(StatutUtilisateur.StatutEnum.Inactif);

        [Fact]
        public void Constructeur_IdentifiantVide_Doit_LeverArgumentException()
        {
            // Arrange
            var id = GuidValide();
            var email = CréerEmail();
            var role = CréerRoleAdmin();
            var statut = CréerStatutActif();
            var identif = "";
            var mdp = "password";

            // Act
            Action act = () => new Utilisateur(id, identif, role, statut, email);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("identifiant")
               .WithMessage("L'identifiant est requis.*");
        }

        [Fact]
        public void Constructeur_MotDePasseVide_Doit_LeverArgumentException()
        {
            // Arrange
            var id = GuidValide();
            var email = CréerEmail();
            var role = CréerRoleAdmin();
            var statut = CréerStatutActif();
            var identif = "user1";
            var mdp = "";

            // Act
            Action act = () => new Utilisateur(id, identif, role, statut, email);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("motDePasse")
               .WithMessage("Le mot de passe ne peut pas être vide.*");
        }

        [Fact]
        public void Constructeur_EmailNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = GuidValide();
            var identif = "user1";
            var mdp = "password";
            var role = CréerRoleAdmin();
            var statut = CréerStatutActif();

            // Act
            Action act = () => new Utilisateur(id, identif,  role, statut, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("email");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = GuidValide();
            var identif = "user1";
            var mdp = "password";
            var role = CréerRoleEnseignant();
            var statut = CréerStatutActif();
            var email = CréerEmail();

            // Act
            var user = new Utilisateur(id, identif, role, statut, email);

            // Assert
            user.Id.Should().Be(id);
            user.Identifiant.Should().Be(identif);
            user.Email.Should().Be(email);
            user.Role.Should().Be(role);
            user.Statut.Should().Be(statut);
           
           
        }

      


        [Fact]
        public void ModifierRole_Doit_ChangerRole()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1", CréerRoleEnseignant(), CréerStatutActif(), CréerEmail());

            // Act
            user.ModifierRole(CréerRoleAdmin());

            // Assert
            user.Role.Valeur.Should().Be(RoleUtilisateur.Role.Admin);
            user.EstAdmin().Should().BeTrue();
            user.EstEnseignant().Should().BeFalse();
        }

        [Fact]
        public void ModifierStatut_Doit_ChangerStatut()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());

            // Act
            user.ModifierStatut(CréerStatutInactif());

            // Assert
            user.Statut.Value.Should().Be(StatutUtilisateur.StatutEnum.Inactif);
        }

        [Fact]
        public void ModifierEmail_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());

            // Act
            Action act = () => user.ModifierEmail(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nouvelEmail");
        }

        [Fact]
        public void ModifierEmail_Valide_Doit_MettreAJourEmail()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());
            var nouvelEmail = new Email("new@example.com");

            // Act
            user.ModifierEmail(nouvelEmail);

            // Assert
            user.Email.Should().Be(nouvelEmail);
        }

        [Fact]
        public void EstAdmin_EstEnseignant_Doit_FonctionnerCorrectement()
        {
            // Arrange
            var user1 = new Utilisateur(GuidValide(), "admin",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());
            var user2 = new Utilisateur(GuidValide(), "enseignant",  CréerRoleEnseignant(), CréerStatutActif(), CréerEmail());

            // Assert
            user1.EstAdmin().Should().BeTrue();
            user1.EstEnseignant().Should().BeFalse();
            user2.EstEnseignant().Should().BeTrue();
            user2.EstAdmin().Should().BeFalse();
        }

        [Fact]
        public void MettreÀJourProfil_IdentifiantVide_Doit_LeverArgumentException()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());

            // Act
            Action act = () => user.MettreÀJourProfil("", new Email("user2@example.com"), CréerRoleEnseignant(), CréerStatutInactif());

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("nouvelIdentifiant")
               .WithMessage("L'identifiant est requis.*");
        }

        [Fact]
        public void MettreÀJourProfil_EmailNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",  CréerRoleAdmin(), CréerStatutActif(), CréerEmail());

            // Act
            Action act = () => user.MettreÀJourProfil("user2", null!, CréerRoleEnseignant(), CréerStatutInactif());

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nouvelEmail");
        }

        [Fact]
        public void MettreÀJourProfil_Valide_Doit_ChangerPropriétés()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1",CréerRoleAdmin(), CréerStatutActif(), CréerEmail());
            var nouvelEmail = new Email("new@example.com");

            // Act
            user.MettreÀJourProfil("user2", nouvelEmail, CréerRoleEnseignant(), CréerStatutInactif());

            // Assert
            user.Identifiant.Should().Be("user2");
            user.Email.Should().Be(nouvelEmail);
            user.Role.Valeur.Should().Be(RoleUtilisateur.Role.Enseignant);
            user.Statut.Value.Should().Be(StatutUtilisateur.StatutEnum.Inactif);
        }

        [Fact]
        public void ToString_Doit_RetournerChaineFormatée()
        {
            // Arrange
            var id = GuidValide();
            var identif = "user1";
            var user = new Utilisateur(id, identif, CréerRoleEnseignant(), CréerStatutActif(), CréerEmail());

            // Act
            var result = user.ToString();

            // Assert
            result.Should().Contain($"Utilisateur: {identif}");
            result.Should().Contain($"Rôle: {user.Role.Valeur}");
            result.Should().Contain($"Statut: {user.Statut.Value}");
            result.Should().Contain($"Email: {user.Email.Value}");
        }

        [Fact]
        public void Supprimer_Doit_PouvoirAppelerSansException()
        {
            // Arrange
            var user = new Utilisateur(GuidValide(), "user1", CréerRoleAdmin(), CréerStatutActif(), CréerEmail());

            // Act
            Action act = () => user.Supprimer();

            // Assert
            act.Should().NotThrow();
        }
    }
}
