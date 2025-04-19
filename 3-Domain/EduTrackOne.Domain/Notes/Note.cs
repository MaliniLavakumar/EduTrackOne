using EduTrackOne.Domain.Abstractions;
using System;

namespace EduTrackOne.Domain.Notes
{
    public class Note : Entity
    {
        public DateTime DateExamen { get; private set; }
        public ValeurNote Valeur { get; private set; }
        public CommentaireEvaluation Commentaire { get; private set; }
        public Guid IdInscription { get; private set; }
        public Guid IdMatiere { get; private set; }

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

        // Méthode pour changer la valeur de la note, avec validation
        public void ModifierValeur(ValeurNote nouvelleValeur)
        {
            if (nouvelleValeur == null)
                throw new ArgumentNullException(nameof(nouvelleValeur));

            // Ajouter ici une logique de validation si nécessaire
            Valeur = nouvelleValeur;
        }

        // Méthode pour ajouter/modifier un commentaire
        public void AjouterCommentaire(CommentaireEvaluation nouveauCommentaire)
        {
            Commentaire = nouveauCommentaire;
        }
    }
}
