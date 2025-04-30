using System;
using System.Collections.Generic;
using System.Linq;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Domain.Eleves
{
    public class Telephone : ValueObject
    {
        public string Value { get; }
        protected Telephone() { }
        public Telephone(string value)
        {
            // Vérification que le numéro commence par +41 et que la longueur est correcte (13 caractères)
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("+41") || value.Length != 12 || !value.Substring(1).All(char.IsDigit))
            {
                throw new ArgumentException("Le numéro de téléphone doit commencer par +41 et être composé de 11 chiffres.");
            }
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
