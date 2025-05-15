using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Inscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Presences
{
    public class Presence : Entity
    {
        public DateTime Date { get; private set; }
        public int Periode { get; private set; }
        public StatutPresence Statut { get; private set; }
        public Guid IdInscription { get; private set; }
        public Inscription Inscription { get; private set; }

        protected Presence() { }
        // Constructeur
        public Presence(Guid id, DateTime date, int periode, StatutPresence statut, Guid idInscription)
            : base(id)
        {
            Date = date;
            Periode = periode;
            Statut = statut ?? throw new ArgumentNullException(nameof(statut));
            IdInscription = idInscription;
        }
        internal void Modifier(DateTime date, int periode, StatutPresence statut)
        {
            Date = date;
            Periode = periode;
            Statut = statut;
        }
        // Affichage de la présence sous forme de chaîne
        public override string ToString()
        {
            return $"Date: {Date.ToShortDateString()}, Periode: {Periode}, Statut: {Statut.Value}";
        }
    }
}
