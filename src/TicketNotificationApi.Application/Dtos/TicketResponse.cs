using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
