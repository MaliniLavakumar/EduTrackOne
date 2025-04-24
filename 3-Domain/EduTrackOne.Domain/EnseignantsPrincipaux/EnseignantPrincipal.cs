using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using System;

namespace EduTrackOne.Domain.EnseignantsPrincipaux
{
    public class EnseignantPrincipal : Entity, IAggregateRoot
    {
        public NomComplet NomComplet { get; private set; }
        public Email Email { get; private set; }

        private readonly List<Classe> _classes = new();
        public IReadOnlyCollection<Classe> Classes => _classes.AsReadOnly();

        protected EnseignantPrincipal() { }


        // Constructeur
        public EnseignantPrincipal(Guid id, NomComplet nomComplet, Email email)
            : base(id)
        {
            NomComplet = nomComplet ?? throw new ArgumentNullException(nameof(nomComplet));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        // Méthode de mise à jour des infos de contact
        public void ModifierCoordonnees(Email nouvelEmail)
        {
            Email = nouvelEmail ?? throw new ArgumentNullException(nameof(nouvelEmail));          
        }

        // Affichage du nom complet
        public string GetFullName() => NomComplet.ToString();
    }
}
