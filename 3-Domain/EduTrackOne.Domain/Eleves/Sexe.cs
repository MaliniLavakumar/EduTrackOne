using System;
using System.Collections.Generic;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Domain.Eleves
{
    public class Sexe : ValueObject
    {
        public string Value { get; }

        public Sexe(string value)
        {
            if (value != "Garçon" && value != "Fille")
                throw new ArgumentException("Le sexe doit être 'Garçon' ou 'Fille'.");
            Value = value;
        }

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
