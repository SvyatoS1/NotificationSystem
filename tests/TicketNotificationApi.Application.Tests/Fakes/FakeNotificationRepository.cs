using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Tests.Fakes
{
    public class FakeNotificationRepository : INotificationRepository
    {
        public Dictionary<Guid, Notification> Notifications { get; } = new();

        public Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                Notifications[notification.Id] = notification;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Notification>> GetByTicketIdAsync(Guid ticketId)
        {
            var result = Notifications.Values.Where(n => n.TicketId == ticketId).ToList();
            return Task.FromResult<IEnumerable<Notification>>(result);
        }

        public Task UpdateAsync(Notification notification)
        {
            Notifications[notification.Id] = notification;
            return Task.CompletedTask;
        }
    }
}
