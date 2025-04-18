using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Presences
{
    public class StatutPresence : ValueObject
    {
        public string Value { get; }

        public StatutPresence(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !(value == "Présent" || value == "Absent" || value == "Retard"))
            {
                throw new ArgumentException("Le statut de présence doit être 'Présent', 'Absent' ou 'Retard'.");
            }

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
