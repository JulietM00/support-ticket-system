using Microsoft.Data.SqlClient;
using SupportTicketSystem.API.Interfaces;
using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Repositories
{
    
    public class AgentRepository : IAgentRepository
    {
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
{
    try
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
    catch (Exception ex)
    {
        throw new Exception($"Error retrieving tickets: {ex.Message}");
    }
}
        private readonly string _connectionString;

        public AgentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Agent>> GetAllAgentsAsync()
        {
            var agents = new List<Agent>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Agents";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                agents.Add(new Agent
                {
                    AgentId   = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName  = reader.GetString(2),
                    Email     = reader.GetString(3),
                    IsActive  = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }
            return agents;
        }

        public async Task<Agent?> GetAgentByIdAsync(int agentId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Agents WHERE agent_id = @AgentId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@AgentId", agentId);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Agent
                {
                    AgentId   = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName  = reader.GetString(2),
                    Email     = reader.GetString(3),
                    IsActive  = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                };
            }
            return null;
        }

        public async Task<Agent> CreateAgentAsync(Agent agent)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Agents (first_name, last_name, email)
                VALUES (@FirstName, @LastName, @Email);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", agent.FirstName);
            command.Parameters.AddWithValue("@LastName",  agent.LastName);
            command.Parameters.AddWithValue("@Email",     agent.Email);

            var newId = await command.ExecuteScalarAsync();
            agent.AgentId = Convert.ToInt32(newId);
            return agent;
        }
    }
}
