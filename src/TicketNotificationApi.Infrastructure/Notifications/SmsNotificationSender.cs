using System;
using System.Collections.Generic;
using System.Text;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Infrastructure.Notifications
{
    public class SmsNotificationSender : INotificationSender
    {
        private const int MinDelayMs = 50;
        private const int MaxDelayMs = 150;
        private const double FailureRate = 0.30;
        public NotificationChannel Channel => NotificationChannel.SMS;

        public async Task<NotificationSendResult> SendAsync(
            Ticket ticket,
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var delay = Random.Shared.Next(MinDelayMs, MaxDelayMs + 1);

            await Task.Delay(delay, cancellationToken);

            var failed = Random.Shared.NextDouble() < FailureRate;

            return failed
                ? NotificationSendResult.Fail("Carrier rejected message")
                : NotificationSendResult.Ok();
        }
    }
}
