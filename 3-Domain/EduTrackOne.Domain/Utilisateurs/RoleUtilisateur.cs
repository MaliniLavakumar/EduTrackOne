using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs
{
    public class RoleUtilisateur : ValueObject
    {
        // Définition de l'énumération interne pour les rôles
        public enum Role
        {
            Admin,
            Enseignant
        }

        public Role Valeur { get; }
        protected RoleUtilisateur() { }

        // Constructeur pour initialiser l'objet valeur
        public RoleUtilisateur(Role role)
        {
            Valeur = role;
        }

        // Constructeur pour accepter une chaîne de caractères (peut-être utile pour la conversion depuis un DB)
        public RoleUtilisateur(string value)
        {
            if (Enum.TryParse(value, true, out Role role))
            {
                Valeur = role;
            }
            else
            {
                throw new ArgumentException("Le rôle doit être 'Admin' ou 'Enseignant'.", nameof(value));
            }
        }

        // Override de la méthode GetEqualityComponents pour comparer les objets valeur
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valeur;
        }

        // Optionnel: Une méthode pour afficher la valeur sous forme de chaîne
        public override string ToString()
        {
            return Valeur.ToString();
        }

        // Méthodes d'aide pour vérifier le rôle
        public bool EstAdmin() => Valeur == Role.Admin;
        public bool EstEnseignant() => Valeur == Role.Enseignant;
    }
}



