using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace EduTrackOne.Domain.Eleves
{
    public class DateNaissance : ValueObject
    {
        public DateTime Value { get; }
        protected DateNaissance() { }      

        public DateNaissance(DateTime value)
        {
            if (value > DateTime.Now)
                throw new ArgumentException("La date de naissance ne peut pas être dans le futur.");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value.ToString("dd/MM/yyyy");
        }
    }
}
