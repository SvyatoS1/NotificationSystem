using Microsoft.AspNetCore.Mvc;
using TicketNotificationApi.Application.Abstractions;
using TicketNotificationApi.Application.Dtos;
using TicketNotificationApi.Application.Notifications;
using TicketNotificationApi.Domain.Entities;

namespace TicketNotificationApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationOrchestrator _notificationOrchestrator;

        public TicketsController(
            ITicketRepository ticketRepository,
            INotificationRepository notificationRepository,
            INotificationOrchestrator notificationOrchestrator)
        {
            _ticketRepository = ticketRepository;
            _notificationRepository = notificationRepository;
            _notificationOrchestrator = notificationOrchestrator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketByIdAsync(Guid id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var notifications = await _notificationRepository.GetByTicketIdAsync(id);
            ticket.Notifications = notifications.ToList();

            return Ok(ticket.ToResponse());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicketAsync([FromBody] CreateTicketRequest request)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _ticketRepository.AddAsync(ticket);

            var notifications = await _notificationOrchestrator.CreateNotificationsForTicketAsync(ticket);
            ticket.Notifications = notifications.ToList();

            return CreatedAtAction(nameof(GetTicketByIdAsync), new { id = ticket.Id }, ticket);
        }

        [HttpPost("{id}/notify")]
        public async Task<IActionResult> NotifyAsync(Guid id)
        {
            try
            {
                var notifications = await _notificationOrchestrator.SendPendingNotificationsAsync(id);

                var ticket = await _ticketRepository.GetByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }
                
                ticket.Notifications = notifications.ToList();

                return Ok(ticket.ToResponse());
            }
            catch (Exception)
            {

                return NotFound();
            }
        }
    }
}
