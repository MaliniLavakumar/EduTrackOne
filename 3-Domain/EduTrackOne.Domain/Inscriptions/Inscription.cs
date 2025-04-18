using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Inscriptions
{

    public class Inscription : Entity
    {
        public DateInscriptionPeriode Periode { get; private set; }
        public Guid IdClasse { get; private set; }
        public Guid IdEleve { get; private set; }

        public Inscription(Guid id, DateInscriptionPeriode periode, Guid idClasse, Guid idEleve)
            : base(id)
        {
            Periode = periode ?? throw new ArgumentNullException(nameof(periode));
            IdClasse = idClasse;
            IdEleve = idEleve;
        }
    }
}


