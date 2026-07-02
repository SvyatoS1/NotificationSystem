using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketNotificationApi.Application.Abstractions
{
    public record NotificationSendResult(bool IsSuccess, string? ErrorMessage = null)
    {
        public static NotificationSendResult Ok() => new(true, null);

        public static NotificationSendResult Fail(string errorMessage) => new(false, errorMessage);
    }
}
