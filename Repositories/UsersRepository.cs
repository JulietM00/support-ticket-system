using Microsoft.Data.SqlClient;
using SupportTicketSystem.API.Interfaces;
using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

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

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId    = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName  = reader.GetString(2),
                    Email     = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Users WHERE user_id = @UserId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId    = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName  = reader.GetString(2),
                    Email     = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                };
            }
            return null;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Users (first_name, last_name, email)
                VALUES (@FirstName, @LastName, @Email);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName",  user.LastName);
            command.Parameters.AddWithValue("@Email",     user.Email);

            var newId = await command.ExecuteScalarAsync();
            user.UserId = Convert.ToInt32(newId);
            return user;
        }
    }
}