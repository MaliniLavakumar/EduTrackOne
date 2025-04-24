using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Notes
{
    public class ValeurNote : ValueObject
    {
        public bool EstAbsent { get; private set; }
        public double? Value { get; private set; }
        protected ValeurNote() { }
        public ValeurNote(double value)
        {
            if (value < 1 || value > 6 || value % 0.5 != 0)
                throw new ArgumentOutOfRangeException(nameof(value), "La note doit être comprise entre 1 et 6.");

            Value = value;
            EstAbsent = false;
        }
        // Factory pour créer l’état « absent »
        public static ValeurNote Absent()
            => new ValeurNote(true);


        private ValeurNote(bool absent)
        {
            EstAbsent = true;
            Value = null;
        }


        // Pour l’affichage
        public override string ToString()
            => EstAbsent
               ? "Absent"
               : Value!.Value.ToString("0.0");

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EstAbsent;
            if (!EstAbsent)
                yield return Value!.Value;
        }

    }
}
