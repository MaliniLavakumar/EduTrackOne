using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Matieres.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Matieres
{
    public class Matiere : Entity, IAggregateRoot
    {
        public NomMatiere Nom { get; private set; }
        protected Matiere() { }

        public Matiere(Guid id, NomMatiere nom) : base(id)
        {
            Nom = nom ?? throw new ArgumentNullException(nameof(nom));
        }
        public static Matiere Creer(Guid id, NomMatiere nom)
        {
            var matiere = new Matiere(id, nom);

            // Ici, on est *dans* le domaine : on peut appeler AddDomainEvent
            matiere.AddDomainEvent(new MatiereCreatedEvent(id, nom.Value));

            return matiere;
        }

        // Méthode pour affichage ou logique métier
        public string GetNom() => Nom.ToString();
    }
}
