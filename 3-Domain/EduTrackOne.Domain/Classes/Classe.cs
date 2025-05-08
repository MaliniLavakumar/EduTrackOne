using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using EduTrackOne.Domain.Classes.Events;
using System.ComponentModel.DataAnnotations;

namespace EduTrackOne.Domain.Classes
{
    public class Classe : Entity, IAggregateRoot
    {
        public NomClasse Nom { get; private set; }
        public AnneeScolaire AnneeScolaire { get; private set; }

        public Guid? IdEnseignantPrincipal { get; private set; }
        public EnseignantPrincipal EnseignantPrincipal { get; private set; }

        // Liste des inscriptions pour cette classe
        private readonly List<Inscription> _inscriptions = new();
        public IReadOnlyCollection<Inscription> Inscriptions => _inscriptions.AsReadOnly();
        [Timestamp]
        public byte[] RowVersion { get; set; }


        protected Classe() { }

        // Constructeur
        public Classe(Guid id, NomClasse nom, AnneeScolaire anneeScolaire)
            : base(id)
        {
            Nom = nom ?? throw new ArgumentNullException(nameof(nom));
            AnneeScolaire = anneeScolaire ?? throw new ArgumentNullException(nameof(anneeScolaire));
            AddDomainEvent(new ClassCreatedEvent(this.Id));
        }

        // Assigner ou changer l'enseignant principal
        public void AssignerEnseignantPrincipal(Guid enseignantId)
        {
            if (enseignantId == Guid.Empty)
                throw new ArgumentException("L'identifiant de l'enseignant est invalide.");

            IdEnseignantPrincipal = enseignantId;
        }

        public void SupprimerEnseignantPrincipal()
        {
            IdEnseignantPrincipal = null;
        }

        // Inscrire un élève (ne pas permettre doublon)
        public Guid InscrireEleve(Guid eleveId, DateInscriptionPeriode periode)
        {
            if (_inscriptions.Count >= 20)
                throw new InvalidOperationException("La classe a atteint sa capacité maximale.");

            if (eleveId == Guid.Empty)
                throw new ArgumentException("L'identifiant de l'élève est invalide.");

            bool dejaInscrit = _inscriptions.Any(i => i.IdEleve == eleveId);
            if (dejaInscrit)
                throw new InvalidOperationException("Cet élève est déjà inscrit dans cette classe.");

            var inscription = Inscription.Creer(
                Guid.NewGuid(),
                this.Id,
                eleveId,
                periode
                );

            _inscriptions.Add(inscription);
            AjouterDomainEvent(new StudentRegisteredEvent(this.Id, eleveId,periode));
            return inscription.Id;
        }

        // Supprimer une inscription d'élève
        public void SupprimerInscription(Guid eleveId)
        {
            var inscription = _inscriptions.FirstOrDefault(i => i.IdEleve == eleveId);
            if (inscription == null)
                throw new InvalidOperationException("L'élève n'est pas inscrit dans cette classe.");

            _inscriptions.Remove(inscription);
            AjouterDomainEvent(new StudentDeletedEvent(inscription.Id, this.Id, eleveId));
        }

        // Rechercher une inscription
        public Inscription? TrouverInscriptionParEleve(Guid eleveId)
        {
            return _inscriptions.FirstOrDefault(i => i.IdEleve == eleveId);
        }
        public void SupprimerClasse()
        {
            AddDomainEvent(new ClassDeletedEvent(this.Id));
        }
        public void AjouterDomainEvent(IDomainEvent domainEvent)
    => AddDomainEvent(domainEvent);

    }
}
