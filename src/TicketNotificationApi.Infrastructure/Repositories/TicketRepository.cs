using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Infrastructure.Repositories
{
    internal class TicketRepository : ITicketRepository
    {
        private readonly ConcurrentDictionary<Guid, Ticket> _tickets = new();
        public Task AddAsync(Ticket ticket)
        {
            _tickets.TryAdd(ticket.Id, ticket);
            return Task.CompletedTask;
        }

        public Task<Ticket?> GetByIdAsync(Guid id)
        {
            _tickets.TryGetValue(id, out Ticket? ticket);
            return Task.FromResult(ticket);
        }
    }
}
