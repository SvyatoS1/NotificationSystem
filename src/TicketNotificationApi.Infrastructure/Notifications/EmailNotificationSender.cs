using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Domain.Entities;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Infrastructure.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        private const int MinDelayMs = 50;
        private const int MaxDelayMs = 150;
        private const double FailureRate = 0.3;

        public NotificationChannel Channel => NotificationChannel.Email;

        public async Task<NotificationSendResult> SendAsync(
            Ticket ticket,
            Notification notification,
            CancellationToken cancellationToken = default)
        {
            var delay = new Random().Next(MinDelayMs, MaxDelayMs);
            await Task.Delay(delay, cancellationToken);

            var failed = Random.Shared.NextDouble() < FailureRate;

            return failed ? NotificationSendResult.Fail("SMTP timeout") : NotificationSendResult.Ok();
        }
    }
}
