namespace SupportTicketSystem.API.DTOs
{
    public class ResolveTicketRequest
    {
        public string ResolvedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}