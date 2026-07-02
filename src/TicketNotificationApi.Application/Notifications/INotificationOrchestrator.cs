using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Notifications
{
    public interface INotificationOrchestrator
    {
        Task<IEnumerable<Notification>> CreateNotificationsForTicketAsync(Ticket ticket);
        Task<IEnumerable<Notification>> SendPendingNotificationsAsync(Guid ticketId);
    }
}
