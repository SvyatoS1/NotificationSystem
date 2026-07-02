using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Entities;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Abstractions
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task<NotificationSendResult> SendAsync(
            Ticket ticket,
            Notification notification,
            CancellationToken cancellationToken = default);
    }
}
