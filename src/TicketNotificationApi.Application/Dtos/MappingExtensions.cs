using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Dtos
{
    public static class MappingExtensions
    {
        public static NotificationResponse ToResponse(this Notification notification)
        {
            return new NotificationResponse(
                notification.Id,
                notification.Channel,
                notification.Status,
                notification.Attempts,
                notification.LastError,
                notification.CreatedAt
            );
        }

        public static TicketResponse ToResponse(this Ticket ticket)
        {
            return new TicketResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Priority,
                ticket.CreatedAt,
                ticket.Notifications.Select(n => n.ToResponse()).ToList()
            );
        }
    }
}
