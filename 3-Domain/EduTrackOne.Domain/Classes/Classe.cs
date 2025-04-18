using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Enseignants;
using EduTrackOne.Domain.Eleves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EduTrackOne.Domain.Classes
{
    public class Classe : Entity, IAggregateRoot
    {
        public NomClasse Nom { get; private set; }
        public AnneeScolaire AnneeScolaire { get; private set; }
        public List<Inscription> Inscriptions { get; private set; }
        public List<Enseignant> Enseignants { get; private set; }

        public Classe(Guid id, NomClasse nom, AnneeScolaire anneeScolaire)
            : base(id)
        {
            Nom = nom;
            AnneeScolaire = anneeScolaire;
            Inscriptions = new List<Inscription>();
            Enseignants = new List<Enseignant>();
        }

        // Ajout d'une inscription à la classe
        public void AjouterInscription(Inscription inscription)
        {
            if (inscription == null)
                throw new ArgumentNullException(nameof(inscription));
            Inscriptions.Add(inscription);
        }

        // Ajout d'un enseignant à la classe
        public void AjouterEnseignant(Enseignant enseignant)
        {
            if (enseignant == null)
                throw new ArgumentNullException(nameof(enseignant));
            Enseignants.Add(enseignant);
        }
    }

}

}
