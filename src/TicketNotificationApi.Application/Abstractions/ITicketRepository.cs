using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Abstractions
{
    public interface ITicketRepository
    {
        Task AddAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(Guid id);
    }
}
