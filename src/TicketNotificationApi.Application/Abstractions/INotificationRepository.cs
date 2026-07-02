using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Abstractions
{
    public interface INotificationRepository
    {
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task<IEnumerable<Notification>> GetByTicketIdAsync(Guid ticketId);
        Task UpdateAsync(Notification notification);
    }
}
