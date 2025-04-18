using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Enseignants
{
    public class Enseignant : Entity, IAggregateRoot
    {
        public NomComplet NomComplet { get; private set; }
        public Email Email { get; private set; }
        public Guid IdClasse { get; private set; }

        public Enseignant(Guid id, NomComplet nomComplet, Email email, Guid idClasse)
            : base(id)
        {
            NomComplet = nomComplet;
            Email = email;
            IdClasse = idClasse;
        }

        // Méthode d'affichage de l'enseignant
        public string GetFullName() => NomComplet.ToString();
        
    }
}

