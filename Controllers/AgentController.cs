using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.API.Interfaces;
using SupportTicketSystem.API.Models;

namespace SupportTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentRepository _agentRepository;

        public AgentsController(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        // GET: api/agents
        [HttpGet]
        public async Task<IActionResult> GetAllAgents()
        {
            var agents = await _agentRepository.GetAllAgentsAsync();
            return Ok(agents);
        }

        // GET: api/agents/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgentById(int id)
        {
            var agent = await _agentRepository.GetAgentByIdAsync(id);
            if (agent == null)
                return NotFound(new { message = $"Agent with ID {id} not found." });
            return Ok(agent);
        }

        // POST: api/agents
        [HttpPost]
        public async Task<IActionResult> CreateAgent([FromBody] Agent agent)
        {
            if (agent == null)
                return BadRequest(new { message = "Invalid agent data." });

            var created = await _agentRepository.CreateAgentAsync(agent);
            return CreatedAtAction(nameof(GetAgentById), new { id = created.AgentId }, created);
        }
    }
}