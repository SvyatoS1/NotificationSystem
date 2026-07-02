using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public int Attempts { get; set; } = 0;
        public string? LastError { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
