using Microsoft.Extensions.DependencyInjection;
using TicketNotificationApi.Application.Notifications;

namespace TicketNotificationApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<INotificationOrchestrator, NotificationOrchestrator>();
            return services;
        }
    }
}
