using EduTrackOne.Domain.Abstractions;
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

        public Matiere(Guid id, NomMatiere nom) : base(id)
        {
            Nom = nom ?? throw new ArgumentNullException(nameof(nom));
        }

        // Méthode utile (ex : pour affichage ou logique métier)
        public string GetNom() => Nom.ToString();
    }
}
