using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Constructeur
            public Eleve(Guid id, NomComplet nomComplet, DateNaissance dateNaissance, Sexe sexe, Adresse adresse, Telephone tel1, Telephone tel2, string noImmatricule)
                : base(id) 
            {
                NomComplet = nomComplet;
                DateNaissance = dateNaissance;
                Sexe = sexe;
                Adresse = adresse;
                Tel1 = tel1;
                Tel2 = tel2;
                NoImmatricule = noImmatricule;
            }
        }
    }


