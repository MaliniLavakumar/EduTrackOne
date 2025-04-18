using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs
{
    public class Utilisateur : Entity
    {
        public string Identifiant { get; private set; }
        public string MotDePasse { get; private set; }
        public RoleUtilisateur Role { get; private set; }
        public StatutUtilisateur Statut { get; private set; }

        // Constructeur
        public Utilisateur(Guid id, string identifiant, string motDePasse, RoleUtilisateur role, StatutUtilisateur statut)
            : base(id)
        {
            Identifiant = identifiant;
            MotDePasse = motDePasse;
            Role = role;
            Statut = statut;
        }

        // Affichage de l'utilisateur sous forme de chaîne
        public override string ToString()
        {
            return $"Identifiant: {Identifiant}, Rôle: {Role.Value}, Statut: {Statut.Value}";
        }
    }

}
