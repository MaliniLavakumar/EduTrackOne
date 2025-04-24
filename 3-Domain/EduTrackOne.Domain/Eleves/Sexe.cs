using System;
using System.Collections.Generic;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Domain.Eleves
{
    public class Sexe : ValueObject
    {
        public SexeType Value { get; }
       protected Sexe() { }
        public Sexe(SexeType value)
        {
            if (!Enum.IsDefined(typeof(SexeType), value))
                throw new ArgumentException("Valeur invalide pour le sexe.");
            Value = value;
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
        public enum SexeType
        {
            Garçon,
            Fille
        }
    }
}