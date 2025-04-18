using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Utilisateurs
{
    public class StatutUtilisateur : ValueObject
    {
        public string Value { get; }

        public StatutUtilisateur(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !(value == "Actif" || value == "Inactif"))
            {
                throw new ArgumentException("Le statut doit être 'Actif' ou 'Inactif'.");
            }

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

}
