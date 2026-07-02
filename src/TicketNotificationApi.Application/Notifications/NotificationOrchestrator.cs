using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Notifications
{
    public class NotificationOrchestrator(
        ITicketRepository ticketRepository,
        INotificationRepository notificationRepository,
        IEnumerable<INotificationSender> notificationSenders) : INotificationOrchestrator
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IEnumerable<INotificationSender> _notificationSenders = notificationSenders;

        public async Task<IEnumerable<Notification>> CreateNotificationsForTicketAsync(Ticket ticket)
        {
            var channels = new[] { NotificationChannel.Email, NotificationChannel.SMS, NotificationChannel.Push };
            var notifications = new List<Notification>();

            foreach (var channel in channels)
            {
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    Channel = channel,
                    Status = NotificationStatus.Pending,
                    Attempts = 0,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }
            await _notificationRepository.AddRangeAsync(notifications);
            return notifications;
        }

        public async Task<IEnumerable<Notification>> SendPendingNotificationsAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new ArgumentException($"Ticket with ID {ticketId} not found.");

            var notifications = await _notificationRepository.GetByTicketIdAsync(ticketId);

            foreach (var notification in notifications)
            {
                if (notification.Status == NotificationStatus.Sent)
                {
                    continue;
                }

                if (notification.Attempts >= 3)
                {
                    continue;
                }

                var sender = _notificationSenders.Single(s => s.Channel == notification.Channel);
                var result = await sender.SendAsync(ticket, notification);

                if (result.IsSuccess)
                {
                    notification.Status = NotificationStatus.Sent;
                    notification.LastError = null;

                }
                else
                {
                    notification.Attempts++;
                    notification.Status = NotificationStatus.Failed;
                    notification.LastError = result.ErrorMessage;
                }

                await _notificationRepository.UpdateAsync(notification);
            }

            return notifications;
        }
    }
}
