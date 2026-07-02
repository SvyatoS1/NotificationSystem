using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketNotificationApi.Domain.Enums;

namespace TicketNotificationApi.Application.Dtos
{
    public record CreateTicketRequest(
        [Required]
        [MinLength(5)]
        string Title,
        string? Description,
        Priority Priority
    );
}
