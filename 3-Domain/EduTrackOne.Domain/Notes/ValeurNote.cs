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
        public decimal Valeur { get; }

        public ValeurNote(decimal valeur)
        {
            if (valeur < 1 || valeur > 6)
                throw new ArgumentOutOfRangeException(nameof(valeur), "La note doit être comprise entre 1 et 6.");

            Valeur = Math.Round(valeur, 1);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Valeur;
        }

        public override string ToString() => Valeur.ToString("0.0");
    }
}
