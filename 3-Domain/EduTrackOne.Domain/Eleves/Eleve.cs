using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves.Events;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata;

namespace EduTrackOne.Domain.Eleves
{
    public class Eleve : Entity, IAggregateRoot
    {
        public NomComplet NomComplet { get; private set; }
        public DateNaissance DateNaissance { get; private set; }
        public Sexe Sexe { get; private set; }
        public Adresse Adresse { get; private set; }
        public Telephone Tel1 { get; private set; }
        public Telephone? Tel2 { get; private set; }
        public Email EmailParent { get; private set; }
        public string NoImmatricule { get; private set; }

        private readonly List<Inscription> _inscriptions = new();
        public IReadOnlyCollection<Inscription> Inscriptions => _inscriptions.AsReadOnly();
        protected Eleve() { }

        // Constructeur
        public Eleve(Guid id, NomComplet nomComplet, DateNaissance dateNaissance, Sexe sexe,
                     Adresse adresse, Email emailParent,  Telephone tel1, Telephone tel2, string noImmatricule)
            : base(id)
        {
            NomComplet = nomComplet ?? throw new ArgumentNullException(nameof(nomComplet));
            DateNaissance = dateNaissance ?? throw new ArgumentNullException(nameof(dateNaissance));
            Sexe = sexe ?? throw new ArgumentNullException(nameof(sexe));
            Adresse = adresse ?? throw new ArgumentNullException(nameof(adresse));
            EmailParent = emailParent ?? throw new ArgumentNullException(nameof(emailParent));
            Tel1 = tel1 ?? throw new ArgumentNullException(nameof(tel1));
            Tel2 = tel2;
            NoImmatricule = noImmatricule ?? throw new ArgumentNullException(nameof(noImmatricule));
            AddDomainEvent(new EleveCreatedEvent(this.Id, this.NoImmatricule));
        }

        // Mettre à jour les infos personnelles
        public void ModifierCoordonnees(Adresse nouvelleAdresse, Telephone tel1, Telephone tel2, Email emailParent)
        {
            Adresse = nouvelleAdresse ?? throw new ArgumentNullException(nameof(nouvelleAdresse));
            Tel1 = tel1 ?? throw new ArgumentNullException(nameof(tel1));
            Tel2 = tel2; //facultatif
            EmailParent = emailParent ?? throw new ArgumentNullException(nameof (emailParent));
        }

        // Affichage nom complet
        public string GetNomComplet() => NomComplet.ToString();
    }
}
