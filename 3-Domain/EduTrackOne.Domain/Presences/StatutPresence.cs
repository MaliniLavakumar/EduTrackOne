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
        public enum StatutEnum
        {
            Present,
            Absent        
        }
        protected StatutPresence() { }
      
        public StatutEnum Value { get; }
        public StatutPresence(StatutEnum value)
        {
            Value = value;
        }
        public override string ToString() => Value.ToString();

        public string Afficher() => Value switch
        {
            StatutEnum.Present => "Présent",
            StatutEnum.Absent => "Absent",
            _ => "Inconnu"
        };

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static StatutPresence FromString(string statut)
        {
            var normalized = statut.Trim().ToLowerInvariant();

            return normalized switch
            {
                "présent" or "present" => new StatutPresence(StatutEnum.Present),
                "absent" => new StatutPresence(StatutEnum.Absent),
                _ => throw new ArgumentException("Statut invalide. Doit être 'Présent' ou 'Absent'.")
            };
        }
    }

}

