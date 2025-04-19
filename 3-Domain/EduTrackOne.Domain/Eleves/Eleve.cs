using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduTrackOne.Domain.Eleves
{
    public class Eleve : Entity, IAggregateRoot
    {
        public NomComplet NomComplet { get; private set; }
        public DateNaissance DateNaissance { get; private set; }
        public Sexe Sexe { get; private set; }
        public Adresse Adresse { get; private set; }
        public Telephone Tel1 { get; private set; }
        public Telephone Tel2 { get; private set; }
        public string NoImmatricule { get; private set; }

        private readonly List<Inscription> _inscriptions = new();
        public IReadOnlyCollection<Inscription> Inscriptions => _inscriptions.AsReadOnly();

        // Constructeur
        public Eleve(Guid id, NomComplet nomComplet, DateNaissance dateNaissance, Sexe sexe,
                     Adresse adresse, Telephone tel1, Telephone tel2, string noImmatricule)
            : base(id)
        {
            NomComplet = nomComplet ?? throw new ArgumentNullException(nameof(nomComplet));
            DateNaissance = dateNaissance ?? throw new ArgumentNullException(nameof(dateNaissance));
            Sexe = sexe;
            Adresse = adresse ?? throw new ArgumentNullException(nameof(adresse));
            Tel1 = tel1 ?? throw new ArgumentNullException(nameof(tel1));
            Tel2 = tel2 ?? throw new ArgumentNullException(nameof(tel2));
            NoImmatricule = noImmatricule ?? throw new ArgumentNullException(nameof(noImmatricule));
        }

        // Méthode métier : ajouter une inscription
        public void AjouterInscription(Inscription inscription)
        {
            if (inscription == null)
                throw new ArgumentNullException(nameof(inscription));

            // Optionnel : vérifier si déjà inscrit dans la même année scolaire ?
            bool dejaInscrit = _inscriptions.Any(i => i.AnneeScolaire == inscription.AnneeScolaire);
            if (dejaInscrit)
                throw new InvalidOperationException("L'élève est déjà inscrit pour cette année scolaire.");

            _inscriptions.Add(inscription);
        }
        

        // Mettre à jour les infos personnelles
        public void ModifierCoordonnees(Adresse nouvelleAdresse, Telephone tel1, Telephone tel2)
        {
            Adresse = nouvelleAdresse ?? throw new ArgumentNullException(nameof(nouvelleAdresse));
            Tel1 = tel1 ?? throw new ArgumentNullException(nameof(tel1));
            Tel2 = tel2 ?? throw new ArgumentNullException(nameof(tel2));
        }

        // Affichage nom complet
        public string GetNomComplet() => NomComplet.ToString();
    }
}
