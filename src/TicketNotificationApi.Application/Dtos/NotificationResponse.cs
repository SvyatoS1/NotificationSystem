using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Dtos
{
    public record NotificationResponse(
        Guid Id,
        NotificationChannel Channel,
        NotificationStatus Status,
        int Attempts,
        string? LastError,
        DateTimeOffset CreatedAt
    );
}
