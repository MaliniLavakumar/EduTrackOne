using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using EduTrackOne.Domain.Utilisateurs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Persistence
{
    public class EduTrackOneDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public EduTrackOneDbContext(DbContextOptions<EduTrackOneDbContext> options)
        : base(options)
        { }
        public EduTrackOneDbContext(DbContextOptions<EduTrackOneDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Classe> Classes { get; set; }
        public DbSet<Eleve> Eleves { get; set; }
        public DbSet<EnseignantPrincipal> EnseignantsPrincipaux { get; set; }
        public DbSet<Inscription> Inscriptions { get; set; }
        public DbSet<Matiere> Matieres { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Presence> Presences { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker.Entries<Entity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            // Nettoyer après publication
            ChangeTracker.Entries<Entity>().ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la relation EnseignantPrincipal -> Classes (1 EnseignantPrincipal, N Classes)
            modelBuilder.Entity<EnseignantPrincipal>(b =>
            {
                b.ToTable("EnseignantsPrincipaux");
                b.HasKey(ep => ep.Id);
                b.OwnsOne(ep => ep.NomComplet, vo =>
                {
                    vo.Property(v => v.Prenom).HasColumnName("Prenom");
                    vo.Property(v => v.Nom).HasColumnName("Nom");
                });
                b.OwnsOne(ep => ep.Email, vo => vo.Property(v => v.Value).HasColumnName("Email"));

                b.HasMany(c => c.Classes)
                 .WithOne(c => c.EnseignantPrincipal)
                 .HasForeignKey(c => c.IdEnseignantPrincipal)
                 .OnDelete(DeleteBehavior.SetNull);
                b.Metadata
                .FindNavigation(nameof(EnseignantPrincipal.Classes))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            // Classe configuration
            modelBuilder.Entity<Classe>(b =>
            {
                b.ToTable("Classes");
                b.HasKey(c => c.Id);
                b.OwnsOne(c => c.Nom, vo => vo.Property(v => v.Value).HasColumnName("Nom"));
                b.OwnsOne(c => c.AnneeScolaire, vo => vo.Property(v => v.Value).HasColumnName("AnneeScolaire"));
                b.Property(c => c.IdEnseignantPrincipal).HasColumnName("IdEnseignantPrincipal");
                // Mapping de la collection via le champ privé _inscriptions
                b.Property(e => e.RowVersion)
                 .IsRowVersion()
                 .HasColumnName("RowVersion")
                 .ValueGeneratedOnAddOrUpdate();
            });

            // Eleve configuration
            modelBuilder.Entity<Eleve>(b =>
            {
                b.ToTable("Eleves");
                b.HasKey(e => e.Id);
                b.Property(e => e.RowVersion)
                .IsRowVersion()
                .HasColumnName("RowVersion")
                .ValueGeneratedOnAddOrUpdate();
                b.OwnsOne(e => e.NomComplet, vo =>
                {
                    vo.Property(v => v.Prenom).HasColumnName("Prenom");
                    vo.Property(v => v.Nom).HasColumnName("Nom");
                });
                b.OwnsOne(e => e.DateNaissance, vo => vo.Property(v => v.Value).HasColumnName("DateNaissance"));
                b.OwnsOne(e => e.Sexe, vo =>
                {
                    vo.Property(v => v.Value)
                        .HasColumnName("Sexe")
                        .HasConversion(
                        v => v.ToString(),
                        v => (Sexe.SexeType)Enum.Parse(typeof(Sexe.SexeType), v)
                        );
                });
                b.OwnsOne(e => e.Adresse, vo =>
                {
                    vo.Property(v => v.Rue).HasColumnName("Rue");
                    vo.Property(v => v.CodePostal).HasColumnName("CodePostal");
                    vo.Property(v => v.Ville).HasColumnName("Ville");
                });
                b.OwnsOne(e => e.EmailParent, vo => vo.Property(v => v.Value).HasColumnName("email"));
                b.OwnsOne(e => e.Tel1, vo => vo.Property(v => v.Value).HasColumnName("Tel1"));
                b.OwnsOne(e => e.Tel2, vo => vo.Property(v => v.Value).HasColumnName("Tel2"));
                b.Property(e => e.NoImmatricule).HasColumnName("NoImmatricule");

            });

            // Inscription configuration
            modelBuilder.Entity<Inscription>(b =>
            {

                b.ToTable("Inscriptions");
                b.HasKey(i => i.Id);
                b.Property(e => e.RowVersion)
                .IsRowVersion()
                .HasColumnName("RowVersion")
                .ValueGeneratedOnAddOrUpdate();

                b.OwnsOne(i => i.Periode, vo =>
                {
                    vo.Property(v => v.DateDebut).HasColumnName("DateDebut");
                    vo.Property(v => v.DateFin).HasColumnName("DateFin");
                });
                b.Property(i => i.IdClasse).HasColumnName("IdClasse");
                b.Property(i => i.IdEleve).HasColumnName("IdEleve");
                b.HasOne<Classe>()              // pas de nav property dans Classe
                 .WithMany()                    // pas de nav property inverse
                 .HasForeignKey(i => i.IdClasse)
                 .OnDelete(DeleteBehavior.Cascade);
                b.HasOne<Eleve>()
                 .WithMany()
                 .HasForeignKey(i => i.IdEleve)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(i => i.Notes)
                 .WithOne(n => n.Inscription)
                 .HasForeignKey(n => n.IdInscription)
                 .OnDelete(DeleteBehavior.Cascade);
                // 1 → N Presences
                b.HasMany(i => i.Presences)
                 .WithOne(p => p.Inscription)
                 .HasForeignKey(p => p.IdInscription)
                 .OnDelete(DeleteBehavior.Cascade);
                b.Metadata
     .FindNavigation(nameof(Inscription.Notes))!
     .SetPropertyAccessMode(PropertyAccessMode.Field);

                b.Metadata
                 .FindNavigation(nameof(Inscription.Presences))!
                 .SetPropertyAccessMode(PropertyAccessMode.Field);

            });

            // Matiere configuration
            modelBuilder.Entity<Matiere>(b =>
            {
                b.ToTable("Matieres");
                b.HasKey(m => m.Id);
                b.OwnsOne(m => m.Nom, vo => vo.Property(v => v.Value).HasColumnName("Nom"));
            });

            // Note configuration
            modelBuilder.Entity<Note>(b =>
            {
                b.ToTable("Notes");
                b.HasKey(n => n.Id);
                b.Property(n => n.DateExamen).HasColumnName("DateExamen");
                b.OwnsOne(n => n.Valeur, vo => vo.Property(v => v.Value).HasColumnName("Valeur"));
                b.OwnsOne(n => n.Commentaire, vo => vo.Property(v => v.Value).HasColumnName("Commentaire"));
                b.Property(n => n.IdInscription).HasColumnName("IdInscription");
                b.Property(n => n.IdMatiere).HasColumnName("IdMatiere");
            });

            // Presence configuration
            modelBuilder.Entity<Presence>(b =>
            {
                b.ToTable("Presences");
                b.HasKey(p => p.Id);
                b.Property(p => p.Date).HasColumnName("Date");
                b.Property(p => p.Periode).HasColumnName("Periode");
                b.OwnsOne(p => p.Statut, vo =>
                {
                    vo.Property(v => v.Value)
                      .HasColumnName("Statut")
                      .HasConversion(
                          v => v.ToString(),                      // enum → string pour la DB
                          v => Enum.Parse<StatutPresence.StatutEnum>(v) // string → enum
                      );
                });
                b.Property(p => p.IdInscription).HasColumnName("IdInscription");
            });

            // Utilisateur configuration
            modelBuilder.Entity<Utilisateur>(b =>
            {
                b.ToTable("Utilisateurs");
                b.HasKey(u => u.Id);
                b.Property(u => u.Identifiant).HasColumnName("Identifiant");
                b.Property(u => u.MotDePasseHash).HasColumnName("MotDePasseHash");
                b.OwnsOne(u => u.Email, vo => vo.Property(v => v.Value).HasColumnName("Email"));
                b.OwnsOne(u => u.Role, vo => vo.Property(v => v.Valeur).HasColumnName("Role"));
                b.OwnsOne(u => u.Statut, vo => vo.Property(v => v.Value).HasColumnName("Statut"));
            });
        }
    }
}

