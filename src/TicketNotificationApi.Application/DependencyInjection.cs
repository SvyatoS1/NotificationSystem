using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Application.Notifications;
using TicketNotificationApi.Domain.Entities;

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
