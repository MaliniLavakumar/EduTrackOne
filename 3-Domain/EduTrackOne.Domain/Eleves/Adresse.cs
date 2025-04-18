using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace EduTrackOne.Domain.Eleves
{
    public class Adresse : ValueObject
    {
        public string Rue { get; }
        public string Ville { get; }
        public string CodePostal { get; }

        public Adresse(string rue, string codePostal, string ville)
        {
            if (string.IsNullOrWhiteSpace(rue) || string.IsNullOrWhiteSpace(codePostal) || string.IsNullOrWhiteSpace(ville))
                throw new ArgumentException("L'adresse doit être complète et valide.");

            Rue = rue;
            Ville = ville;
            CodePostal = codePostal;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Rue;
            yield return CodePostal;
            yield return Ville;
        }

        public override string ToString()
        {
            return $"{Rue}, {CodePostal}, {Ville}";
        }
    }
}
