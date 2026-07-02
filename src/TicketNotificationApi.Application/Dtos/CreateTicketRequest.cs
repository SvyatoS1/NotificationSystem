using System.ComponentModel.DataAnnotations;
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
