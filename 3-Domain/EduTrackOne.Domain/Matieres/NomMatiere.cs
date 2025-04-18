using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Matieres
{
    public class NomMatiere : ValueObject
    {
        public string Value { get; }

        public NomMatiere(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Le nom de la matière est requis.", nameof(value));

            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
