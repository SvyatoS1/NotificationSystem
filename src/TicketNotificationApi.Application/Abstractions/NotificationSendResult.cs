namespace TicketNotificationApi.Application.Abstractions
{
    public record NotificationSendResult(bool IsSuccess, string? ErrorMessage = null)
    {
        public static NotificationSendResult Ok() => new(true, null);

        public static NotificationSendResult Fail(string errorMessage) => new(false, errorMessage);
    }
}
