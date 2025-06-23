using System;
using FluentAssertions;
using Xunit;
using EduTrackOne.Domain.Notes;

namespace EduTrackOne.Tests.DomainTests.Entities
{
    public class NoteTests
    {
        // Méthode utilitaire pour générer un GUID valide
        private Guid GuidValide() => Guid.NewGuid();

        // Méthode utilitaire pour créer une ValeurNote valide
        private ValeurNote CréerValeurNote(double valeur) => new ValeurNote(valeur);

        // Méthode utilitaire pour créer un CommentaireEvaluation (ou null si non nécessaire)
        // On passe null ici même si le paramètre n'est pas nullable, 
        // le constructeur ne vérifie pas explicitement commentatire pour null.
        private CommentaireEvaluation? CréerCommentaire() => null;

        [Fact]
        public void Constructeur_Valide_Doit_CréerInstance()
        {
            // Arrange
            var id = GuidValide();
            var dateExamen = new DateTime(2025, 01, 15);
            var valeurNote = CréerValeurNote(4.5);
            var commentaire = CréerCommentaire();
            var idInscription = GuidValide();
            var idMatiere = GuidValide();

            // Act
            var note = new Note(id, dateExamen, valeurNote, commentaire!, idInscription, idMatiere);

            // Assert
            note.Id.Should().Be(id);            // Identifiant hérité de Entity
            note.DateExamen.Should().Be(dateExamen);
            note.Valeur.Should().Be(valeurNote);
            note.Commentaire.Should().BeNull();
            note.IdInscription.Should().Be(idInscription);
            note.IdMatiere.Should().Be(idMatiere);
        }

        [Fact]
        public void Constructeur_ValeurNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var id = GuidValide();
            var dateExamen = DateTime.UtcNow.Date;
            ValeurNote valeur = null!;
            var commentaire = CréerCommentaire();
            var idInscription = GuidValide();
            var idMatiere = GuidValide();

            // Act
            Action act = () => new Note(id, dateExamen, valeur!, commentaire!, idInscription, idMatiere);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("valeur");
        }

        [Fact]
        public void Modifier_DateExamenDefaut_Doit_LeverArgumentException()
        {
            // Arrange
            var note = new Note(
                GuidValide(),
                new DateTime(2025, 01, 01),
                CréerValeurNote(3.0),
                CréerCommentaire()!,
                GuidValide(),
                GuidValide()
            );

            // Act
            Action act = () => note.Modifier(default, GuidValide(), CréerValeurNote(4.0), null);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithParameterName("nouvelleDateExamen")
               .WithMessage("Date d'examen invalide.*");
        }

        [Fact]
        public void Modifier_ValeurNull_Doit_LeverArgumentNullException()
        {
            // Arrange
            var note = new Note(
                GuidValide(),
                new DateTime(2025, 01, 01),
                CréerValeurNote(3.0),
                CréerCommentaire()!,
                GuidValide(),
                GuidValide()
            );

            // Act
            Action act = () => note.Modifier(new DateTime(2025, 02, 01), GuidValide(), null!, null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("nouvelleValeur");
        }

        [Fact]
        public void Modifier_Valide_Doit_MettreAJourPropriétés()
        {
            // Arrange
            var id = GuidValide();
            var idInscription = GuidValide();
            var idMatiereOld = GuidValide();
            var note = new Note(
                id,
                new DateTime(2025, 01, 01),
                CréerValeurNote(3.0),
                CréerCommentaire()!,
                idInscription,
                idMatiereOld
            );

            var nouvelleDate = new DateTime(2025, 03, 01);
            var nouvelleMatiere = GuidValide();
            var nouvelleValeur = CréerValeurNote(5.0);
            var nouveauCommentaire = new CommentaireEvaluation("Bon travail");

            // Act
            note.Modifier(nouvelleDate, nouvelleMatiere, nouvelleValeur, nouveauCommentaire);

            // Assert
            note.DateExamen.Should().Be(nouvelleDate);
            note.IdMatiere.Should().Be(nouvelleMatiere);
            note.Valeur.Should().Be(nouvelleValeur);
            note.Commentaire.Should().Be(nouveauCommentaire);
        }
    }
}
