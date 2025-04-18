using EduTrackOne.Domain.Abstractions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EduTrackOne.Domain.Classes
{
    public class AnneeScolaire : ValueObject
    {
        public string Value { get; }

        public AnneeScolaire(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("L'année scolaire ne peut pas être vide.");

            // Validation du format "YYYY-YYYY"
            var regex = new Regex(@"^\d{4}-\d{4}$");
            if (!regex.IsMatch(value))
                throw new ArgumentException("L'année scolaire doit suivre le format attendu (ex: 2024-2025).");

            var years = value.Split('-');
            int firstYear = int.Parse(years[0]);
            int secondYear = int.Parse(years[1]);

            if (secondYear != firstYear + 1)
                throw new ArgumentException("Les années scolaires doivent être consécutives (ex: 2024-2025).");

            if (firstYear >= secondYear)
                throw new ArgumentException("La première année doit être inférieure à la deuxième année.");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
