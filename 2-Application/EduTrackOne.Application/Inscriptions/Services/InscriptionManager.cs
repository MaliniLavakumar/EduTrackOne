using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Inscriptions.Services
{
    public class InscriptionManager : IInscriptionManager
    {
        public void AjouterNote(Inscription inscription, Note note)
        {
            if (inscription == null)
                throw new ArgumentNullException(nameof(inscription));
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            if (note.IdInscription != inscription.Id)
                throw new InvalidOperationException("La note ne correspond pas à cette inscription.");

            inscription.AjouterNote(note);
        }

        public void MarquerPresence(Inscription inscription, Presence presence)
        {
            if (inscription == null)
                throw new ArgumentNullException(nameof(inscription));
            if (presence == null)
                throw new ArgumentNullException(nameof(presence));

            if (presence.IdInscription != inscription.Id)
                throw new InvalidOperationException("La présence ne correspond pas à cette inscription.");

            inscription.MarquerPresence(presence);
        }
        public void ModifierNote(
       Inscription inscription,
       Guid noteId,
       DateTime nouvelleDateExamen,
       Guid nouvelleMatiereId,
       ValeurNote nouvelleValeur,
       CommentaireEvaluation? nouveauCommentaire)
        {
            if (inscription == null) throw new ArgumentNullException(nameof(inscription));
            inscription.ModifierNote(noteId, nouvelleDateExamen, nouvelleMatiereId, nouvelleValeur, nouveauCommentaire);
        }

        public void ModifierPresence(
            Inscription inscription,
            Guid presenceId,
            DateTime nouvelleDate,
            int nouvellePeriode,
            StatutPresence nouveauStatut)
        {
            if (inscription == null) throw new ArgumentNullException(nameof(inscription));
            inscription.ModifierPresence(presenceId, nouvelleDate, nouvellePeriode, nouveauStatut);
        }
    }
}
