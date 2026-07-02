using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Infrastructure.Repositories
{
    internal class NotificationRepository : INotificationRepository
    {
        private readonly ConcurrentDictionary<Guid, Notification> _notifications = new();
        public Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                _notifications.TryAdd(notification.Id, notification);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Notification>> GetByTicketIdAsync(Guid ticketId)
        {
            var filteredNotifications = _notifications.Values
                .Where(n => n.TicketId == ticketId)
                .ToList();

            return Task.FromResult<IEnumerable<Notification>>(filteredNotifications);
        }

        public Task UpdateAsync(Notification notification)
        {
            _notifications.AddOrUpdate(
                notification.Id,
                notification,
                (key, existingNotification) => notification);
            return Task.CompletedTask;
        }
    }
}
