using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Inscriptions;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class ClasseTests
    {
        private NomClasse CréerNomClasse() => new NomClasse("4P/01-2024");
        private AnneeScolaire CréerAnneeScolaire() => new AnneeScolaire("2024-2025");
        private DateInscriptionPeriode CréerPeriode() => new DateInscriptionPeriode(DateTime.UtcNow.Date.AddDays(-1), null);

        [Fact]
        public void Constructeur_NomNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            AnneeScolaire annee = CréerAnneeScolaire();

            // Act
            Action act = () => new Classe(id, null!, annee);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nom");
        }

        [Fact]
        public void Constructeur_AnneeNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            NomClasse nom = CréerNomClasse();

            // Act
            Action act = () => new Classe(id, nom, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("anneeScolaire");
        }

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            NomClasse nom = CréerNomClasse();
            AnneeScolaire annee = CréerAnneeScolaire();

            // Act
            var classe = new Classe(id, nom, annee);

            // Assert
            classe.Id.Should().Be(id);
            classe.Nom.Should().Be(nom);
            classe.AnneeScolaire.Should().Be(annee);
            classe.Inscriptions.Should().BeEmpty();
            // Aucun événement n'est vérifié ici puisque le service d'événements n'est pas en place
        }

        [Fact]
        public void AssignerEnseignantPrincipal_GuidVide_Doit_LeverArgumentException()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());

            // Act
            Action act = () => classe.AssignerEnseignantPrincipal(Guid.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'identifiant de l'enseignant est invalide.");
        }

        [Fact]
        public void AssignerEnseignantPrincipal_Valide_Doit_MettreAJourIdEnseignant()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var enseignantId = Guid.NewGuid();

            // Act
            classe.AssignerEnseignantPrincipal(enseignantId);

            // Assert
            classe.IdEnseignantPrincipal.Should().Be(enseignantId);
        }

        [Fact]
        public void SupprimerEnseignantPrincipal_Doit_RetirerIdEnseignant()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var enseignantId = Guid.NewGuid();
            classe.AssignerEnseignantPrincipal(enseignantId);

            // Act
            classe.SupprimerEnseignantPrincipal();

            // Assert
            classe.IdEnseignantPrincipal.Should().BeNull();
        }

        [Fact]
        public void InscrireEleve_GuidEleveVide_Doit_LeverArgumentException()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var periode = CréerPeriode();

            // Act
            Action act = () => classe.InscrireEleve(Guid.Empty, periode);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'identifiant de l'élève est invalide.");
        }

        [Fact]
        public void InscrireEleve_DéjàInscrit_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var eleveId = Guid.NewGuid();
            var periode = CréerPeriode();
            classe.InscrireEleve(eleveId, periode);

            // Act
            Action act = () => classe.InscrireEleve(eleveId, periode);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Cet élève est déjà inscrit dans cette classe.");
        }

        [Fact]
        public void InscrireEleve_CapacitéMaximale_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var periode = CréerPeriode();

            // Inscrire 20 élèves différents
            for (int i = 0; i < 20; i++)
            {
                var idEleve = Guid.NewGuid();
                classe.InscrireEleve(idEleve, periode);
            }

            // Act
            Action act = () => classe.InscrireEleve(Guid.NewGuid(), periode);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("La classe a atteint sa capacité maximale.");
        }

        [Fact]
        public void InscrireEleve_Valide_Doit_AjouterInscription()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var eleveId = Guid.NewGuid();
            var periode = CréerPeriode();

            // Act
            var inscriptionId = classe.InscrireEleve(eleveId, periode);

            // Assert
            var inscription = classe.Inscriptions.Single();
            inscription.Id.Should().Be(inscriptionId);
            inscription.IdEleve.Should().Be(eleveId);
            classe.Inscriptions.Should().HaveCount(1);
            // Aucun événement n'est vérifié ici
        }

        [Fact]
        public void TrouverInscriptionParEleve_NonInscrit_Doit_RetournerNull()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var rechercheId = Guid.NewGuid();

            // Act
            var result = classe.TrouverInscriptionParEleve(rechercheId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TrouverInscriptionParEleve_Inscrit_Doit_RetournerInscription()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var eleveId = Guid.NewGuid();
            var periode = CréerPeriode();
            var insId = classe.InscrireEleve(eleveId, periode);

            // Act
            var result = classe.TrouverInscriptionParEleve(eleveId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(insId);
        }

        [Fact]
        public void SupprimerInscription_NonExistante_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var eleveId = Guid.NewGuid();

            // Act
            Action act = () => classe.SupprimerInscription(eleveId);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("L'élève n'est pas inscrit dans cette classe.");
        }

        [Fact]
        public void SupprimerInscription_Existante_Doit_RetirerInscription()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());
            var eleveId = Guid.NewGuid();
            var periode = CréerPeriode();
            classe.InscrireEleve(eleveId, periode);

            // Act
            classe.SupprimerInscription(eleveId);

            // Assert
            classe.Inscriptions.Should().BeEmpty();
            // Aucun événement n'est vérifié ici
        }

        [Fact]
        public void SupprimerClasse_Doit_RetirerTouteLogiqueSansPublierEvent()
        {
            // Arrange
            var classe = new Classe(Guid.NewGuid(), CréerNomClasse(), CréerAnneeScolaire());

            // Act
            classe.SupprimerClasse();

            // Assert
            // Ici on ne vérifie pas d'événement, juste qu'on peut appeler la méthode sans exception
            classe.Should().NotBeNull();
        }
    }
}
