namespace SupportTicketSystem.API.Models
{
    public class TicketHistory
    {
        public int HistoryId { get; set; }
        public int TicketId { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public string? OldStatus { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }
}