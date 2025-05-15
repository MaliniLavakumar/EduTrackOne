using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Abstractions
{
   public interface IInscriptionManager
    {
        void AjouterNote(Inscription inscription, Note note);
        void MarquerPresence(Inscription inscription, Presence presence);
        void ModifierNote(
        Inscription inscription,
        Guid noteId,
        DateTime nouvelleDateExamen,
        Guid nouvelleMatiereId,
        ValeurNote nouvelleValeur,
        CommentaireEvaluation? nouveauCommentaire);

        void ModifierPresence(
            Inscription inscription,
            Guid presenceId,
            DateTime nouvelleDate,
            int nouvellePeriode,
            StatutPresence nouveauStatut);
    }
}
