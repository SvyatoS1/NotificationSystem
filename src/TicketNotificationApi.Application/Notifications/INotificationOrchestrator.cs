using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Notifications
{
    public interface INotificationOrchestrator
    {
        Task<IEnumerable<Notification>> CreateNotificationsForTicketAsync(Ticket ticket);
        Task<IEnumerable<Notification>> SendPendingNotificationsAsync(Guid ticketId);
    }
}
