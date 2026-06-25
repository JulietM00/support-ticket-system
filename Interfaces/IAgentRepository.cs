using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Interfaces
{
    public interface IAgentRepository
    {
        Task<IEnumerable<Agent>> GetAllAgentsAsync();
        Task<Agent?> GetAgentByIdAsync(int agentId);
        Task<Agent> CreateAgentAsync(Agent agent);
    }
}
