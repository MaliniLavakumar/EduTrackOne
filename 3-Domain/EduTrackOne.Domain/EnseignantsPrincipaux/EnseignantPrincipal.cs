using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using System;

namespace EduTrackOne.Domain.EnseignantsPrincipaux
{
    public class EnseignantPrincipal : Entity
    {
        public NomComplet NomComplet { get; private set; }
        public Email Email { get; private set; }
        public Telephone Telephone { get; private set; }

        // Constructeur
        public EnseignantPrincipal(Guid id, NomComplet nomComplet, Email email, Telephone telephone)
            : base(id)
        {
            NomComplet = nomComplet ?? throw new ArgumentNullException(nameof(nomComplet));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Telephone = telephone ?? throw new ArgumentNullException(nameof(telephone));
        }

        // Méthode de mise à jour des infos de contact
        public void ModifierCoordonnees(Email nouvelEmail, Telephone nouveauTelephone)
        {
            Email = nouvelEmail ?? throw new ArgumentNullException(nameof(nouvelEmail));
            Telephone = nouveauTelephone ?? throw new ArgumentNullException(nameof(nouveauTelephone));
        }

        // Affichage du nom complet
        public string GetFullName() => NomComplet.ToString();
    }
}
