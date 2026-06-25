using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Interfaces
{
    public interface ITicketRepository
    {
        // Get all tickets
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        
        // Get a single ticket by ID
        Task<Ticket?> GetTicketByIdAsync(int ticketId);
        
        // Create a new ticket
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        
        // Update an existing ticket
        Task<Ticket?> UpdateTicketAsync(int ticketId, Ticket ticket);
        
        // Delete a ticket
        Task<bool> DeleteTicketAsync(int ticketId);

        // Business logic
        Task<int> AssignTicketToAgentAsync(int ticketId);
        Task<string> ResolveTicketAsync(int ticketId, string resolvedBy, string notes);
        Task<string> EscalateOverdueTicketsAsync();
        Task<object> GetDashboardStatsAsync();
    }
}