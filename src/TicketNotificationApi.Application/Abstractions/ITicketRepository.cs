using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Abstractions
{
    public interface ITicketRepository
    {
        Task AddAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(Guid id);
    }
}
