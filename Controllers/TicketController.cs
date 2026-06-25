using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.API.DTOs;
using SupportTicketSystem.API.Interfaces;
using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/tickets/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound(new { message = $"Ticket with ID {id} not found." });
            return Ok(ticket);
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            if (ticket == null)
                return BadRequest(new { message = "Invalid ticket data." });

            var created = await _ticketRepository.CreateTicketAsync(ticket);
            return CreatedAtAction(nameof(GetTicketById), new { id = created.TicketId }, created);
        }

        // PUT: api/tickets/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            var updated = await _ticketRepository.UpdateTicketAsync(id, ticket);
            if (updated == null)
                return NotFound(new { message = $"Ticket with ID {id} not found." });
            return Ok(updated);
        }

        // DELETE: api/tickets/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var deleted = await _ticketRepository.DeleteTicketAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Ticket with ID {id} not found." });
            return Ok(new { message = $"Ticket {id} deleted successfully." });
        }

        // POST: api/tickets/1/assign
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id)
        {
            var agentId = await _ticketRepository.AssignTicketToAgentAsync(id);
            return Ok(new { message = $"Ticket {id} assigned to Agent {agentId} successfully." });
        }

        // POST: api/tickets/1/resolve
        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id, [FromBody] ResolveTicketRequest request)
        {
            var result = await _ticketRepository.ResolveTicketAsync(id, request.ResolvedBy, request.Notes);
            return Ok(new { message = result });
        }

        // POST: api/tickets/escalate
        [HttpPost("escalate")]
        public async Task<IActionResult> EscalateTickets()
        {
            var result = await _ticketRepository.EscalateOverdueTicketsAsync();
            return Ok(new { message = result });
        }

        // GET: api/tickets/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _ticketRepository.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}