﻿using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.Domain.Inscriptions
{
    public class Inscription : Entity, IAggregateRoot
    {
        public DateInscriptionPeriode Periode { get; private set; }
        public Guid IdClasse { get; private set; }
        public Guid IdEleve { get; private set; }
        private Classe _classe;
        public Classe Classe => _classe;
        private Eleve _eleve;
        public Eleve Eleve => _eleve;

        // Liste des notes de l'élève pour cette inscription
        private readonly List<Note> _notes = new();
        public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

        // Liste des présences de l'élève pour cette inscription
        private readonly List<Presence> _presences = new();
        public IReadOnlyCollection<Presence> Presences => _presences.AsReadOnly();
        [Timestamp]
        public byte[] RowVersion { get; set; }

        protected Inscription() { }

        // Constructeur de l'inscription
        public Inscription(Guid id, DateInscriptionPeriode periode, Guid idClasse, Guid idEleve)
            : base(id)
        {
            Periode = periode ?? throw new ArgumentNullException(nameof(periode));
            IdClasse = idClasse;
            IdEleve = idEleve;
        }

        public static Inscription Creer(Guid id, Guid idClasse, Guid idEleve, DateInscriptionPeriode periode)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant ne peut pas être vide.", nameof(id));
            if (idClasse == Guid.Empty)
                throw new ArgumentException("L'identifiant de la classe ne peut pas être vide.", nameof(idClasse));
            if (idEleve == Guid.Empty)
                throw new ArgumentException("L'identifiant de l'élève ne peut pas être vide.", nameof(idEleve));
            // Déléguation au constructeur privé avec l'ordre des paramètres correct
            return new Inscription(id, periode, idClasse, idEleve);
        }

        #region Méthodes pour injection de références externes (lecture)
        public void SetClasse(Classe classe)
        {
            _classe = classe ?? throw new ArgumentNullException(nameof(classe));
        }

        public void SetEleve(Eleve eleve)
        {
            _eleve = eleve ?? throw new ArgumentNullException(nameof(eleve));
        }
        #endregion
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
