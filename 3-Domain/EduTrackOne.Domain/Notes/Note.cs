using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using System;

namespace EduTrackOne.Domain.Notes
{
    public class Note : Entity
    {
        public DateTime DateExamen { get; private set; }
        public ValeurNote Valeur { get; private set; }
        public CommentaireEvaluation? Commentaire { get; private set; }
        public Guid IdInscription { get; private set; }
        public Inscription Inscription { get; private set; }
        public Guid IdMatiere { get; private set; }
        protected Note() { }

        // Constructeur
        public Note(Guid id, DateTime dateExamen, ValeurNote valeur, CommentaireEvaluation commentaire, Guid idInscription, Guid idMatiere)
            : base(id)
        {
            DateExamen = dateExamen;
            Valeur = valeur ?? throw new ArgumentNullException(nameof(valeur));
            Commentaire = commentaire;
            IdInscription = idInscription;
            IdMatiere = idMatiere;
        }

        internal void Modifier(DateTime nouvelleDateExamen, Guid nouvelleIdMatiere, ValeurNote nouvelleValeur, CommentaireEvaluation? commentaire)
        {
            if (nouvelleDateExamen == default)
                throw new ArgumentException("Date d'examen invalide.", nameof(nouvelleDateExamen));

            if (nouvelleValeur == null)
                throw new ArgumentNullException(nameof(nouvelleValeur));
            DateExamen = nouvelleDateExamen;
            IdMatiere = nouvelleIdMatiere;
            Valeur = nouvelleValeur;
            Commentaire = commentaire;
        }

    }
}
