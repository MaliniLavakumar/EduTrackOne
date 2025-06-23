using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class EleveTests
    {
        // Méthodes utilitaires pour créer des Value Objects valides
        private NomComplet CréerNomComplet() => new NomComplet("Sophie", "Durand");
        private DateNaissance CréerDateNaissance() => new DateNaissance(new DateTime(2010, 5, 12));
        private Sexe CréerSexe() => new Sexe(Sexe.SexeType.Fille);
        private Adresse CréerAdresse() => new Adresse("Rue de la Paix 10", "1000", "Lausanne");
        private Email CréerEmail() => new Email("parent@example.com");
        private Telephone CréerTelephone1() => new Telephone("+41791234567");
        private Telephone CréerTelephone2() => new Telephone("+41790000000");
        private string CréerNoImmatricule() => "ELV-2025-001";

        [Fact]
        public void Constructeur_Null_NomComplet_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, null!, dateNaiss, sexe, adresse, email, tel1, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("nomComplet");
        }

        [Fact]
        public void Constructeur_Null_DateNaissance_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, nomComplet, null!, sexe, adresse, email, tel1, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("dateNaissance");
        }

        [Fact]
        public void Constructeur_Null_Sexe_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, nomComplet, dateNaiss, null!, adresse, email, tel1, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("sexe");
        }

        [Fact]
        public void Constructeur_Null_Adresse_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, nomComplet, dateNaiss, sexe, null!, email, tel1, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("adresse");
        }

        [Fact]
        public void Constructeur_Null_EmailParent_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, nomComplet, dateNaiss, sexe, adresse, null!, tel1, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("emailParent");
        }

        [Fact]
        public void Constructeur_Null_Tel1_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            Action act = () => new Eleve(id, nomComplet, dateNaiss, sexe, adresse, email, null!, tel2, immatricule);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("tel1");
        }

        [Fact]
        public void Constructeur_Null_NoImmatricule_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();

            // Act
            Action act = () => new Eleve(id, nomComplet, dateNaiss, sexe, adresse, email, tel1, tel2, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("noImmatricule");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            // Act
            var eleve = new Eleve(id, nomComplet, dateNaiss, sexe, adresse, email, tel1, tel2, immatricule);

            // Assert
            eleve.Id.Should().Be(id);
            eleve.NomComplet.Should().Be(nomComplet);
            eleve.DateNaissance.Should().Be(dateNaiss);
            eleve.Sexe.Should().Be(sexe);
            eleve.Adresse.Should().Be(adresse);
            eleve.EmailParent.Should().Be(email);
            eleve.Tel1.Should().Be(tel1);
            eleve.Tel2.Should().Be(tel2);
            eleve.NoImmatricule.Should().Be(immatricule);
            eleve.Inscriptions.Should().BeEmpty();
        }

        [Fact]
        public void GetNomComplet_Doit_RetournerPrenomEspaceNom()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomComplet = CréerNomComplet();
            var dateNaiss = CréerDateNaissance();
            var sexe = CréerSexe();
            var adresse = CréerAdresse();
            var email = CréerEmail();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var immatricule = CréerNoImmatricule();

            var eleve = new Eleve(id, nomComplet, dateNaiss, sexe, adresse, email, tel1, tel2, immatricule);

            // Act
            var result = eleve.GetNomComplet();

            // Assert
            result.Should().Be("Sophie Durand");
        }

        [Fact]
        public void ModifierCoordonnees_NouvelleAdresseNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var eleve = new Eleve(id, CréerNomComplet(), CréerDateNaissance(), CréerSexe(), CréerAdresse(), CréerEmail(), CréerTelephone1(), CréerTelephone2(), CréerNoImmatricule());

            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();
            var email = CréerEmail();

            // Act
            Action act = () => eleve.ModifierCoordonnees(null!, tel1, tel2, email);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("nouvelleAdresse");
        }

        [Fact]
        public void ModifierCoordonnees_NouvelTel1Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var eleve = new Eleve(id, CréerNomComplet(), CréerDateNaissance(), CréerSexe(), CréerAdresse(), CréerEmail(), CréerTelephone1(), CréerTelephone2(), CréerNoImmatricule());

            var nouvelleAdresse = CréerAdresse();
            var tel2 = CréerTelephone2();
            var email = CréerEmail();

            // Act
            Action act = () => eleve.ModifierCoordonnees(nouvelleAdresse, null!, tel2, email);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("tel1");
        }

        [Fact]
        public void ModifierCoordonnees_NouvelEmailNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var eleve = new Eleve(id, CréerNomComplet(), CréerDateNaissance(), CréerSexe(), CréerAdresse(), CréerEmail(), CréerTelephone1(), CréerTelephone2(), CréerNoImmatricule());

            var nouvelleAdresse = CréerAdresse();
            var tel1 = CréerTelephone1();
            var tel2 = CréerTelephone2();

            // Act
            Action act = () => eleve.ModifierCoordonnees(nouvelleAdresse, tel1, tel2, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("emailParent");
        }

        [Fact]
        public void ModifierCoordonnees_Valide_Doit_MettreAJourCoordonnees()
        {
            // Arrange
            var id = Guid.NewGuid();
            var eleve = new Eleve(id, CréerNomComplet(), CréerDateNaissance(), CréerSexe(), CréerAdresse(), CréerEmail(), CréerTelephone1(), CréerTelephone2(), CréerNoImmatricule());

            var nouvelleAdresse = new Adresse("Avenue de la Gare 5", "1200", "Genève");
            var nouveauTel1 = new Telephone("+41790000001");
            var nouveauTel2 = CréerTelephone2();
            var nouveauEmail = new Email("nouveauparent@example.com");

            // Act
            eleve.ModifierCoordonnees(nouvelleAdresse, nouveauTel1, nouveauTel2, nouveauEmail);

            // Assert
            eleve.Adresse.Should().Be(nouvelleAdresse);
            eleve.Tel1.Should().Be(nouveauTel1);
            eleve.Tel2.Should().Be(nouveauTel2);
            eleve.EmailParent.Should().Be(nouveauEmail);
        }
    }
}
