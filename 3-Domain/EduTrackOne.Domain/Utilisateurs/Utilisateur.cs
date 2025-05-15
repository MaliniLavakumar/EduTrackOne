using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Utilisateurs.Events;
using System;
using System.Security.Cryptography;
using System.Text;

namespace EduTrackOne.Domain.Utilisateurs
{ 

    public class Utilisateur : Entity, IAggregateRoot
    {
        public string Identifiant { get; private set; }
        public string MotDePasseHash { get; private set; }
        public RoleUtilisateur Role { get; private set; }
        public StatutUtilisateur Statut { get; private set; }
        public Email Email { get; private set; }
        protected Utilisateur() { }       
        public Utilisateur(
            Guid id,
            string identifiant,
            string motDePasse,
            RoleUtilisateur role,
            StatutUtilisateur statut,
            Email email)
            : base(id)
        {
            Identifiant = string.IsNullOrWhiteSpace(identifiant)
                ? throw new ArgumentException("L'identifiant est requis.", nameof(identifiant))
                : identifiant;

            MotDePasseHash = HashMotDePasse(motDePasse);
            Role = role;
            Statut = statut;
            Email = email ?? throw new ArgumentNullException(nameof(email));

            AddDomainEvent(new UserCreatedEvent(Id, Identifiant, Email.Value));
        }

        // Vérifier qu’un mot de passe clair correspond au hash stocké
        public bool VerifierMotDePasse(string motDePasse)
            => MotDePasseHash == HashMotDePasse(motDePasse);

        // Authentifier un utilisateur (login)
        public void Authentifier()
        {
            AddDomainEvent(new UserLoggedInEvent(Id, DateTime.UtcNow));
        }

        // Mettre à jour le mot de passe (nécessite l’ancien)
        public void ModifierMotDePasse(string ancien, string nouveau)
        {
            if (!VerifierMotDePasse(ancien))
                throw new InvalidOperationException("Ancien mot de passe incorrect.");

            MotDePasseHash = HashMotDePasse(nouveau);
            AddDomainEvent(new PasswordChangedEvent(Id));
        }

        // Changer de rôle
        public void ModifierRole(RoleUtilisateur nouveauRole)
        {
            Role = nouveauRole;
        }

        // Changer de statut
        public void ModifierStatut(StatutUtilisateur nouveauStatut)
        {
            Statut = nouveauStatut;
        }

        // Mettre à jour l’email
        public void ModifierEmail(Email nouvelEmail)
        {
            Email = nouvelEmail ?? throw new ArgumentNullException(nameof(nouvelEmail));
        }

        // Helpers de rôle
        public bool EstAdmin() => Role.Valeur == RoleUtilisateur.Role.Admin;
        public bool EstEnseignant() => Role.Valeur == RoleUtilisateur.Role.Enseignant;

        public override string ToString()
        {
            return $"Utilisateur: {Identifiant} | Rôle: {Role.Valeur} | Statut: {Statut.Value} | Email: {Email.Value}";
        }

        //--- Implémentation privée du hash---
        private static string HashMotDePasse(string motDePasse)
        {
            if (string.IsNullOrEmpty(motDePasse))
                throw new ArgumentException("Le mot de passe ne peut pas être vide.", nameof(motDePasse));

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(motDePasse);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
        public void MettreÀJourProfil(
    string nouvelIdentifiant,
    Email nouvelEmail,
    RoleUtilisateur nouveauRole,
    StatutUtilisateur nouveauStatut)
        {
            Identifiant = string.IsNullOrWhiteSpace(nouvelIdentifiant)
                ? throw new ArgumentException("L'identifiant est requis.", nameof(nouvelIdentifiant))
                : nouvelIdentifiant;

            Email = nouvelEmail ?? throw new ArgumentNullException(nameof(nouvelEmail));
            Role = nouveauRole;
            Statut = nouveauStatut;

            AddDomainEvent(new UserUpdatedEvent(Id, Identifiant, Email.Value, DateTime.UtcNow));
        }
        public void Supprimer()
        {
            AddDomainEvent(new UserDeletedEvent(Id, Identifiant));
        }


    }
}
