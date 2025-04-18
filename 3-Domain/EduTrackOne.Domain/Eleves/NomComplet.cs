using System;
using System.Collections.Generic;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Domain.Eleves
{
    public class NomComplet : ValueObject
    {
        public string Prenom { get; }
        public string Nom { get; }

        public NomComplet(string prenom, string nom)
        {
            if (string.IsNullOrWhiteSpace(prenom) || string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le prénom et le nom doivent être valides.");

            Prenom = prenom;
            Nom = nom;
        }

        public override string ToString() => $"{Prenom} {Nom}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Prenom;
            yield return Nom;
        }
    }
}
