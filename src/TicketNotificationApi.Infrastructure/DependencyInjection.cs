using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Infrastructure.Notifications;
using TicketNotificationApi.Infrastructure.Repositories;

namespace TicketNotificationApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ITicketRepository, TicketRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();

            services.AddSingleton<INotificationSender, EmailNotificationSender>();
            services.AddSingleton<INotificationSender, SmsNotificationSender>();
            services.AddSingleton<INotificationSender, PushNotificationSender>();

            return services;
        }
    }
}
