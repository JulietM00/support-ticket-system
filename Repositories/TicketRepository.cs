using Microsoft.Data.SqlClient;
using SupportTicketSystem.API.Interfaces;
using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        
        private readonly string _connectionString;


        public TicketRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get all tickets
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            var tickets = new List<Ticket>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Tickets";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tickets.Add(new Ticket
                {
                    TicketId    = reader.GetInt32(0),
                    Title       = reader.GetString(1),
                    Description = reader.GetString(2),
                    Status      = reader.GetString(3),
                    Priority    = reader.GetString(4),
                    UserId      = reader.GetInt32(5),
                    AgentId     = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    CreatedAt   = reader.GetDateTime(7),
                    UpdatedAt   = reader.GetDateTime(8),
                    ResolvedAt  = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
                });
            }
            return tickets;
        }

        // Get ticket by ID
        public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Tickets WHERE ticket_id = @TicketId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TicketId", ticketId);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Ticket
                {
                    TicketId    = reader.GetInt32(0),
                    Title       = reader.GetString(1),
                    Description = reader.GetString(2),
                    Status      = reader.GetString(3),
                    Priority    = reader.GetString(4),
                    UserId      = reader.GetInt32(5),
                    AgentId     = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    CreatedAt   = reader.GetDateTime(7),
                    UpdatedAt   = reader.GetDateTime(8),
                    ResolvedAt  = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
                };
            }
            return null;
        }

        // Create a new ticket
        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Tickets (title, description, status, priority, user_id, agent_id)
                VALUES (@Title, @Description, @Status, @Priority, @UserId, @AgentId);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title",       ticket.Title);
            command.Parameters.AddWithValue("@Description", ticket.Description);
            command.Parameters.AddWithValue("@Status",      ticket.Status);
            command.Parameters.AddWithValue("@Priority",    ticket.Priority);
            command.Parameters.AddWithValue("@UserId",      ticket.UserId);
            command.Parameters.AddWithValue("@AgentId",     (object?)ticket.AgentId ?? DBNull.Value);

            var newId = await command.ExecuteScalarAsync();
            ticket.TicketId = Convert.ToInt32(newId);
            return ticket;
        }

        // Update a ticket
        public async Task<Ticket?> UpdateTicketAsync(int ticketId, Ticket ticket)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE Tickets SET
                    title       = @Title,
                    description = @Description,
                    status      = @Status,
                    priority    = @Priority,
                    agent_id    = @AgentId,
                    updated_at  = GETDATE()
                WHERE ticket_id = @TicketId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title",       ticket.Title);
            command.Parameters.AddWithValue("@Description", ticket.Description);
            command.Parameters.AddWithValue("@Status",      ticket.Status);
            command.Parameters.AddWithValue("@Priority",    ticket.Priority);
            command.Parameters.AddWithValue("@AgentId",     (object?)ticket.AgentId ?? DBNull.Value);
            command.Parameters.AddWithValue("@TicketId",    ticketId);

            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0 ? ticket : null;
        }

        // Delete a ticket
        public async Task<bool> DeleteTicketAsync(int ticketId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Tickets WHERE ticket_id = @TicketId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TicketId", ticketId);

            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

// Auto assign ticket to agent
        public async Task<int> AssignTicketToAgentAsync(int ticketId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_AssignTicketToAgent", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TicketId", ticketId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        // Resolve a ticket
        public async Task<string> ResolveTicketAsync(int ticketId, string resolvedBy, string notes)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_ResolveTicket", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TicketId",   ticketId);
            command.Parameters.AddWithValue("@ResolvedBy", resolvedBy);
            command.Parameters.AddWithValue("@Notes",      notes);

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "Resolved";
        }

        // Escalate overdue tickets
        public async Task<string> EscalateOverdueTicketsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_EscalateOverdueTickets", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            var result = await command.ExecuteScalarAsync();
            return result?.ToString() ?? "Escalation completed";
        }

        // Get dashboard stats
        public async Task<object> GetDashboardStatsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("sp_GetDashboardStats", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            using var reader = await command.ExecuteReaderAsync();

            var stats = new Dictionary<string, int>();

            if (await reader.ReadAsync())
            {
                stats["totalTickets"]      = reader.GetInt32(0);
                stats["openTickets"]       = reader.GetInt32(1);
                stats["inProgressTickets"] = reader.GetInt32(2);
                stats["resolvedTickets"]   = reader.GetInt32(3);
                stats["closedTickets"]     = reader.GetInt32(4);
                stats["criticalTickets"]   = reader.GetInt32(5);
            }
            return stats;
        }
    }
}