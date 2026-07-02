using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Dtos
{
    public record TicketResponse(
        Guid Id,
        string Title,
        string? Description,
        Priority Priority,
        DateTimeOffset CreatedAt,
        List<NotificationResponse> Notifications
    );
}
