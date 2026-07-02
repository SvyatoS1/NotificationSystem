using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Tests.Fakes
{
    public class FakeTicketRepository : ITicketRepository
    {
        public Dictionary<Guid, Ticket> Tickets { get; } = new();

        public Task AddAsync(Ticket ticket)
        {
            Tickets[ticket.Id] = ticket;
            return Task.CompletedTask;
        }

        public Task<Ticket?> GetByIdAsync(Guid id)
        {
            Tickets.TryGetValue(id, out Ticket? ticket);
            return Task.FromResult(ticket);
        }
    }
}
