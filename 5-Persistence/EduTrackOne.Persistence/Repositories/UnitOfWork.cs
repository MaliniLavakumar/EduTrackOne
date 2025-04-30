using EduTrackOne.Domain.Abstractions;
using MediatR;
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
            int result = await _dbContext.SaveChangesAsync(cancellationToken);

            await DispatchDomainEventsAsync(cancellationToken);

            return result;
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
