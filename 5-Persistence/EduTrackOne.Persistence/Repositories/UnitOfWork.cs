using EduTrackOne.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EduTrackOne.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EduTrackOneDbContext _dbContext;
        private readonly IMediator _mediator;
        public UnitOfWork(EduTrackOneDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await _dbContext.SaveChangesAsync(cancellationToken);

                await DispatchDomainEventsAsync(cancellationToken);

                return result;
            }
            catch (DbUpdateConcurrencyException ex) 
            {
                foreach (var entry in ex.Entries)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = await entry.GetDatabaseValuesAsync();

                    Console.WriteLine("Conflit de mise à jour détecté !");
                    if (databaseValues != null)
                    {
                        foreach (var property in proposedValues.Properties)
                        {
                            var proposed = proposedValues[property];
                            var database = databaseValues[property];
                            if (!Equals(proposed, database))
                            {
                                Console.WriteLine($" - Propriété '{property.Name}': proposée = {proposed}, en base = {database}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(" - L'entité n'existe plus en base.");
                    }
                }

                // Ici tu pourrais choisir de relancer, retourner une erreur custom, ou gérer différemment
                throw;
            }
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            // Trouver tous les agrégats qui ont des Domain Events
            var domainEntities = _dbContext.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            foreach (var entity in domainEntities)
            {
                foreach (var domainEvent in entity.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                entity.ClearDomainEvents();
            }
        }

    }
}
