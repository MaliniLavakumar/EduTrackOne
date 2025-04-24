using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using System;
using System.Collections.Generic;

namespace EduTrackOne.Domain.Inscriptions
{
    public class Inscription : Entity
    {
        public DateInscriptionPeriode Periode { get; private set; }
        public Guid IdClasse { get; private set; }
        public Guid IdEleve { get; private set; }
        public Classe Classe { get; private set; }
        public Eleve Eleve { get; private set; }

        // Liste des notes de l'élève pour cette inscription
        private readonly List<Note> _notes = new();
        public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

        // Liste des présences de l'élève pour cette inscription
        private readonly List<Presence> _presences = new();
        public IReadOnlyCollection<Presence> Presences => _presences.AsReadOnly();

        protected Inscription() { }

        // Constructeur de l'inscription
        public Inscription(Guid id, DateInscriptionPeriode periode, Guid idClasse, Guid idEleve)
            : base(id)
        {
            Periode = periode ?? throw new ArgumentNullException(nameof(periode));
            IdClasse = idClasse;
            IdEleve = idEleve;
        }

        // Méthode pour ajouter une note à l'élève
        public void AjouterNote(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            if (note.IdInscription != this.Id)
                throw new InvalidOperationException("La note ne correspond pas à cette inscription.");

            _notes.Add(note);
        }
        public void ModifierNote(Guid idNote, DateTime nouvelleDateExamen, Guid nouvelleIdMatiere, ValeurNote nouvelleValeur, CommentaireEvaluation? nouveauCommentaire)
        {
            var note = _notes.FirstOrDefault(n => n.Id == idNote);

            if (note == null)
                throw new InvalidOperationException("Note introuvable pour cette inscription.");
            bool doublon = _notes.Any(n =>
            n.Id != idNote &&
            n.IdMatiere == nouvelleIdMatiere &&
            n.DateExamen.Date == nouvelleDateExamen.Date);

            if (doublon)

            {
                throw new InvalidOperationException(
                    "Une autre note pour cette matière à cette date existe déjà.");
            }

            note.Modifier(nouvelleDateExamen, nouvelleIdMatiere, nouvelleValeur, nouveauCommentaire);
        }
        public void SupprimerNote(Guid idNote)
        {
            var note = _notes.FirstOrDefault(n => n.Id == idNote);

            if (note == null)
                throw new InvalidOperationException("Note introuvable pour cette inscription.");

            _notes.Remove(note);
        }



        // Méthode pour marquer la présence de l'élève
        public void MarquerPresence(Presence presence)
        {
            if (presence == null)
                throw new ArgumentNullException(nameof(presence));

            if (presence.IdInscription != this.Id)
                throw new InvalidOperationException("La présence ne correspond pas à cette inscription.");

            bool dejaMarque = _presences.Any(p =>
                p.Date.Date == presence.Date.Date &&
                p.Periode == presence.Periode);
            if (dejaMarque)
                throw new InvalidOperationException(
                    "La présence pour cette date et cette période a déjà été enregistrée.");

            _presences.Add(presence);
        }
    }
}
