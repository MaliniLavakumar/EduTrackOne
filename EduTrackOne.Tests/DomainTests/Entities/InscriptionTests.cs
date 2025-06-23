using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class InscriptionTests
    {
        // Méthodes utilitaires
        private Guid GuidValide() => Guid.NewGuid();

        private DateInscriptionPeriode CréerPeriode() =>
            new DateInscriptionPeriode(
                DateTime.UtcNow.Date.AddDays(-10),
                DateTime.UtcNow.Date.AddDays(10)
            );

        private Classe CréerClasse() =>
            new Classe(
                GuidValide(),
                new NomClasse("4P/01-2024"),
                new AnneeScolaire("2024-2025")
            );

        private Eleve CréerEleve() =>
            new Eleve(
                GuidValide(),
                new NomComplet("Test", "Eleve"),
                new DateNaissance(new DateTime(2010, 1, 1)),
                new Sexe(Sexe.SexeType.Garçon),
                new Adresse("Rue Exemple 1", "1000", "Lausanne"),
                new Email("parent@test.com"),
                new Telephone("+41791234567"),
                new Telephone("+41790000000"),
                "ELV-001"
            );

        // Adaptation : signature de Note = (Guid id, DateTime dateExamen, ValeurNote valeur, CommentaireEvaluation commentaire, Guid idInscription, Guid idMatiere)
        private Note CréerNote(Guid inscriptionId, Guid matiereId, double valeur, DateTime dateExamen) =>
            new Note(
                GuidValide(),               // id
                dateExamen,                 // dateExamen
                new ValeurNote(valeur),     // valeur
                null!,                      // commentaire (null accepté)
                inscriptionId,              // idInscription
                matiereId                   // idMatiere
            );

        private Presence CréerPresence(Guid inscriptionId, DateTime date, int periode, StatutPresence.StatutEnum statutEnum) =>
            new Presence(
                GuidValide(),
                date,
                periode,
                new StatutPresence(statutEnum),
                inscriptionId
               
            );

        [Fact]
        public void Creer_IdVide_Doit_LeverArgumentException()
        {
            // Arrange
            var idVide = Guid.Empty;
            var idClasse = GuidValide();
            var idEleve = GuidValide();
            var periode = CréerPeriode();

            // Act
            Action act = () => Inscription.Creer(idVide, idClasse, idEleve, periode);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("id");
        }

        [Fact]
        public void Creer_IdClasseVide_Doit_LeverArgumentException()
        {
            // Arrange
            var id = GuidValide();
            var idClasse = Guid.Empty;
            var idEleve = GuidValide();
            var periode = CréerPeriode();

            // Act
            Action act = () => Inscription.Creer(id, idClasse, idEleve, periode);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("idClasse");
        }

        [Fact]
        public void Creer_IdEleveVide_Doit_LeverArgumentException()
        {
            // Arrange
            var id = GuidValide();
            var idClasse = GuidValide();
            var idEleve = Guid.Empty;
            var periode = CréerPeriode();

            // Act
            Action act = () => Inscription.Creer(id, idClasse, idEleve, periode);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("idEleve");
        }

        [Fact]
        public void Creer_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = GuidValide();
            var idClasse = GuidValide();
            var idEleve = GuidValide();
            var periode = CréerPeriode();

            // Act
            var inscription = Inscription.Creer(id, idClasse, idEleve, periode);

            // Assert
            inscription.Id.Should().Be(id);
            inscription.IdClasse.Should().Be(idClasse);
            inscription.IdEleve.Should().Be(idEleve);
            inscription.Periode.Should().Be(periode);
            inscription.Notes.Should().BeEmpty();
            inscription.Presences.Should().BeEmpty();
        }

        [Fact]
        public void SetClasse_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());

            // Act
            Action act = () => inscription.SetClasse(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("classe");
        }

        [Fact]
        public void SetClasse_Valide_Doit_AssignerInstance()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var classe = CréerClasse();

            // Act
            inscription.SetClasse(classe);

            // Assert
            inscription.Classe.Should().Be(classe);
        }

        [Fact]
        public void SetEleve_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());

            // Act
            Action act = () => inscription.SetEleve(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("eleve");
        }

        [Fact]
        public void SetEleve_Valide_Doit_AssignerInstance()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var eleve = CréerEleve();

            // Act
            inscription.SetEleve(eleve);

            // Assert
            inscription.Eleve.Should().Be(eleve);
        }

        [Fact]
        public void AjouterNote_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());

            // Act
            Action act = () => inscription.AjouterNote(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("note");
        }

        [Fact]
        public void AjouterNote_IdInscriptionMismatch_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var mauvaiseNote = CréerNote(
                GuidValide(),                   // idInscription différent
                GuidValide(),
                3.5,
                DateTime.UtcNow.Date
            );

            // Act
            Action act = () => inscription.AjouterNote(mauvaiseNote);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("La note ne correspond pas à cette inscription.");
        }

        [Fact]
        public void AjouterNote_Valide_Doit_AjouterÀLaListe()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var note = CréerNote(inscription.Id, GuidValide(), 5.0, DateTime.UtcNow.Date);

            // Act
            inscription.AjouterNote(note);

            // Assert
            inscription.Notes.Should().ContainSingle(n => n.Id == note.Id);
        }

        [Fact]
        public void ModifierNote_NoteInexistante_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var idNoteInexistante = GuidValide();

            // Act
            Action act = () => inscription.ModifierNote(
                idNoteInexistante,
                DateTime.UtcNow.Date,
                GuidValide(),
                new ValeurNote(4.0),
                null
            );

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Note introuvable pour cette inscription.");
        }

        [Fact]
        public void ModifierNote_Doublon_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var matiereId = GuidValide();
            var note1 = CréerNote(inscription.Id, matiereId, 4.0, new DateTime(2025, 1, 1));
            var note2 = CréerNote(inscription.Id, matiereId, 3.0, new DateTime(2025, 2, 1));
            inscription.AjouterNote(note1);
            inscription.AjouterNote(note2);

            // Act
            Action act = () => inscription.ModifierNote(
                note2.Id,
                new DateTime(2025, 1, 1),  // même date que note1
                matiereId,
                new ValeurNote(5.0),
                null
            );

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Une autre note pour cette matière à cette date existe déjà.");
        }

        [Fact]
        public void ModifierNote_Valide_Doit_ModifierNote()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var matiereIdOld = GuidValide();
            var note = CréerNote(inscription.Id, matiereIdOld, 2.5, new DateTime(2025, 1, 1));
            inscription.AjouterNote(note);

            var nouvelleDate = new DateTime(2025, 3, 1);
            var nouvelleMatiere = GuidValide();
            var nouvelleValeur = new ValeurNote(5.0);
            CommentaireEvaluation? commentaire = null;

            // Act
            inscription.ModifierNote(
                note.Id,
                nouvelleDate,
                nouvelleMatiere,
                nouvelleValeur,
                commentaire
            );

            // Assert
            var noteModifiée = inscription.Notes.Single(n => n.Id == note.Id);
            noteModifiée.DateExamen.Should().Be(nouvelleDate);
            noteModifiée.IdMatiere.Should().Be(nouvelleMatiere);
            noteModifiée.Valeur.Value.Should().Be(nouvelleValeur.Value);
        }

        [Fact]
        public void SupprimerNote_Inexistante_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var idNoteInexistante = GuidValide();

            // Act
            Action act = () => inscription.SupprimerNote(idNoteInexistante);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Note introuvable pour cette inscription.");
        }

        [Fact]
        public void SupprimerNote_Existante_Doit_RetirerNote()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var note = CréerNote(inscription.Id, GuidValide(), 3.0, DateTime.UtcNow.Date);
            inscription.AjouterNote(note);

            // Act
            inscription.SupprimerNote(note.Id);

            // Assert
            inscription.Notes.Should().BeEmpty();
        }

        [Fact]
        public void MarquerPresence_Null_Doit_LeverArgumentNullException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());

            // Act
            Action act = () => inscription.MarquerPresence(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("presence");
        }

        [Fact]
        public void MarquerPresence_MismatchId_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var presence = CréerPresence(GuidValide(), DateTime.UtcNow.Date, 1, StatutPresence.StatutEnum.Present);

            // Act
            Action act = () => inscription.MarquerPresence(presence);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("La présence ne correspond pas à cette inscription.");
        }

        [Fact]
        public void MarquerPresence_Doublon_Doit_LeverInvalidOperationException()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var date = DateTime.UtcNow.Date;
            var presence1 = CréerPresence(inscription.Id, date, 1, StatutPresence.StatutEnum.Present);
            inscription.MarquerPresence(presence1);

            var presence2 = CréerPresence(inscription.Id, date, 1, StatutPresence.StatutEnum.Absent);

            // Act
            Action act = () => inscription.MarquerPresence(presence2);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("La présence pour cette date et cette période a déjà été enregistrée.");
        }

        [Fact]
        public void MarquerPresence_Valide_Doit_AjouterPrésence()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var presence = CréerPresence(inscription.Id, DateTime.UtcNow.Date, 1, StatutPresence.StatutEnum.Present);

            // Act
            inscription.MarquerPresence(presence);

            // Assert
            inscription.Presences.Should().ContainSingle(p => p.Id == presence.Id);
        }

        [Fact]
        public void CalculerMoyennesParMatiere_SansNotes_Doit_RetournerDictionnaireVide()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());

            // Act
            var result = inscription.CalculerMoyennesParMatiere();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void CalculerMoyennesParMatiere_AvecNotesValides_Doit_RetournerMoyennesCorrectes()
        {
            // Arrange
            var inscription = Inscription.Creer(GuidValide(), GuidValide(), GuidValide(), CréerPeriode());
            var matiereA = GuidValide();
            var matiereB = GuidValide();

            var note1 = CréerNote(inscription.Id, matiereA, 4.0, new DateTime(2025, 1, 1));
            var note2 = CréerNote(inscription.Id, matiereA, 2.0, new DateTime(2025, 2, 1));
            var note3 = CréerNote(inscription.Id, matiereB, 5.0, new DateTime(2025, 3, 1));

            inscription.AjouterNote(note1);
            inscription.AjouterNote(note2);
            inscription.AjouterNote(note3);

            // Act
            var result = inscription.CalculerMoyennesParMatiere();

            // Assert
            result.Should().HaveCount(2);
            result[matiereA].Should().BeApproximately((4.0 + 2.0) / 2.0, 0.0001);
            result[matiereB].Should().Be(5.0);
        }
    }
}
