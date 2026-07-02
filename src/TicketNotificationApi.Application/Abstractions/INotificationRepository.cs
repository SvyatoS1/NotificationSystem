using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Application.Abstractions
{
    public interface INotificationRepository
    {
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task<IEnumerable<Notification>> GetByTicketIdAsync(Guid ticketId);
        Task UpdateAsync(Notification notification);
    }
}
