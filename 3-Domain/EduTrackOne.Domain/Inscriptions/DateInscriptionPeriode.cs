using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions
{
    public class DateInscriptionPeriode : ValueObject
    {
        public DateTime DateDebut { get; private set; }
        public DateTime? DateFin { get; private set; }

        public DateInscriptionPeriode(DateTime dateDebut, DateTime? dateFin)
        {
            if (dateFin.HasValue && dateFin.Value < dateDebut)
                throw new ArgumentException("La date de fin ne peut pas être avant la date de début.");

            DateDebut = dateDebut;
            DateFin = dateFin;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DateDebut;
            yield return DateFin;
        }
        public bool EstActive()
        {
            return !DateFin.HasValue || DateFin.Value >= DateTime.UtcNow.Date;
        }

    }

}
