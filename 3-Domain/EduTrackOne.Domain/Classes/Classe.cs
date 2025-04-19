using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EduTrackOne.Domain.Classes
{
    public class Classe : Entity, IAggregateRoot
    {
        public NomClasse Nom { get; private set; }
        public AnneeScolaire AnneeScolaire { get; private set; }

        public Guid? IdEnseignantPrincipal { get; private set; }

        // Liste des inscriptions pour cette classe
        private readonly List<Inscription> _inscriptions = new();
        public IReadOnlyCollection<Inscription> Inscriptions => _inscriptions.AsReadOnly();

        // Constructeur
        public Classe(Guid id, NomClasse nom, AnneeScolaire anneeScolaire)
            : base(id)
        {
            Nom = nom ?? throw new ArgumentNullException(nameof(nom));
            AnneeScolaire = anneeScolaire ?? throw new ArgumentNullException(nameof(anneeScolaire));
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
        public void InscrireEleve(Guid eleveId, DateInscriptionPeriode periode)
        {
            if (eleveId == Guid.Empty)
                throw new ArgumentException("L'identifiant de l'élève est invalide.");

            bool dejaInscrit = _inscriptions.Any(i => i.IdEleve == eleveId);
            if (dejaInscrit)
                throw new InvalidOperationException("Cet élève est déjà inscrit dans cette classe.");

            if (Inscriptions.Count >= 20)
                throw new InvalidOperationException("La classe a atteint sa capacité maximale.");

            var inscription = new Inscription(Guid.NewGuid(), periode, this.Id, eleveId);
            _inscriptions.Add(inscription);
        }

        // Supprimer une inscription d'élève
        public void SupprimerInscription(Guid eleveId)
        {
            var inscription = _inscriptions.FirstOrDefault(i => i.IdEleve == eleveId);
            if (inscription == null)
                throw new InvalidOperationException("L'élève n'est pas inscrit dans cette classe.");

            _inscriptions.Remove(inscription);
        }

        // Rechercher une inscription
        public Inscription? TrouverInscriptionParEleve(Guid eleveId)
        {
            return _inscriptions.FirstOrDefault(i => i.IdEleve == eleveId);
        }
        public Eleve? ObtenirEleve(Guid idEleve)
        {
            var inscription = _inscriptions.FirstOrDefault(i => i.IdEleve == idEleve);
            if (inscription != null)
            {
                // Retourne l'élève associé à l'inscription dans cette classe
                return inscription.Eleve;
            }
            return null; // Élève non trouvé dans cette classe
        }

    }
}
