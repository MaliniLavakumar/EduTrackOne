using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using System;
using System.Collections.Generic;

namespace EduTrackOne.Domain.Inscriptions
{
    public class Inscription : Entity, IAggregateRoot
    {
        public DateInscriptionPeriode Periode { get; private set; }
        public Guid IdClasse { get; private set; }
        public Guid IdEleve { get; private set; }

        // Liste des notes de l'élève pour cette inscription
        private readonly List<Note> _notes = new();
        public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

        // Liste des présences de l'élève pour cette inscription
        private readonly List<Presence> _presences = new();
        public IReadOnlyCollection<Presence> Presences => _presences.AsReadOnly();

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

        // Méthode pour marquer la présence de l'élève
        public void MarquerPresence(Presence presence)
        {
            if (presence == null)
                throw new ArgumentNullException(nameof(presence));

            if (presence.IdInscription != this.Id)
                throw new InvalidOperationException("La présence ne correspond pas à cette inscription.");

            _presences.Add(presence);
        }
    }
}
