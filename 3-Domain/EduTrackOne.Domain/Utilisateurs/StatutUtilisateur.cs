using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace EduTrackOne.Domain.Utilisateurs
{
    public class StatutUtilisateur : ValueObject
    {
        // Enum pour le statut de l'utilisateur
        public enum StatutEnum
        {
            Actif,
            Inactif
        }

        public StatutEnum Value { get; }

        // Constructeur vide nécessaire pour EF Core
        protected StatutUtilisateur() { }

        // Constructeur avec validation
        public StatutUtilisateur(StatutEnum value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        // Méthode statique pour créer l'objet à partir d'une chaîne
        public static StatutUtilisateur FromString(string statut)
        {
            if (Enum.TryParse<StatutEnum>(statut, true, out var result))
            {
                return new StatutUtilisateur(result);
            }

            throw new ArgumentException("Statut invalide. Doit être 'Actif' ou 'Inactif'.");
        }
    }
}
