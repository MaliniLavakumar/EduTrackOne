using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Notes
{

    public class Note : Entity, IAggregateRoot
    {
        public DateTime DateExamen { get; private set; }
        public ValeurNote Valeur { get; private set; }
        public CommentaireEvaluation Commentaire { get; private set; }
        public Guid IdInscription { get; private set; }
        public Guid IdMatiere { get; private set; }

        public Note(Guid id, DateTime dateExamen, ValeurNote valeur, CommentaireEvaluation commentaire, Guid idInscription, Guid idMatiere)
            : base(id)
        {
            DateExamen = dateExamen;
            Valeur = valeur;
            Commentaire = commentaire;
            IdInscription = idInscription;
            IdMatiere = idMatiere;
        }
    }
}


