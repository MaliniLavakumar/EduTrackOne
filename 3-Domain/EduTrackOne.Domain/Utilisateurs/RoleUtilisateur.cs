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
        public string Value { get; }

        public RoleUtilisateur(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !(value == "Admin" || value == "Enseignant"))
            {
                throw new ArgumentException("Le rôle doit être 'Admin' ou 'Enseignant'.");
            }

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}
