using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EduTrackOne.Domain.Classes
{
    public class NomClasse : ValueObject
    {
        public string Value { get; }
        protected NomClasse() { }
        public NomClasse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Le nom de la classe ne peut pas être vide.");

            // Format attendu : exemple "4P/01-2024"
            var regex = new Regex(@"^\d+[A-Za-z]/\d{2}-\d{4}$");
            if (!regex.IsMatch(value))
                throw new ArgumentException("Le nom de la classe doit suivre le format attendu (ex: 4P/01-2024).");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
