// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using System.ComponentModel.DataAnnotations;

namespace Aesclea_Back_End_.Models
{
    public enum TicketStatus
    {
        Open,
        InProgress,
        Waiting,
        Resolved,
        Closed
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TicketCategory
    {
        Technical,
        Billing,
        General,
        Feature,
        Bug
    }

    public class SupportTicket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string? AssignedAgentId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public TicketCategory Category { get; set; } = TicketCategory.General;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public string? ResolutionNote { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public User? AssignedAgent { get; set; }
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
        public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    }

    public class TicketMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TicketId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsFromAgent { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public SupportTicket Ticket { get; set; } = null!;
        public User Sender { get; set; } = null!;
    }

    public class TicketAttachment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TicketId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public SupportTicket Ticket { get; set; } = null!;
        public TicketMessage Message { get; set; } = null!;
    }

    // DTOs for API requests/responses
    public class CreateTicketRequest
    {
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public TicketCategory Category { get; set; } = TicketCategory.General;
    }

    public class UpdateTicketRequest
    {
        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
        public TicketCategory? Category { get; set; }
        public string? AssignedAgentId { get; set; }
        public string? ResolutionNote { get; set; }
    }

    public class SendMessageRequest
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class TicketResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public SupportTicket? Ticket { get; set; }
        public IEnumerable<SupportTicket>? Tickets { get; set; }
    }

    public class TicketMessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TicketMessage? TicketMessage { get; set; }
        public IEnumerable<TicketMessage>? Messages { get; set; }
    }

    public class TicketStatsResponse
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int MyAssignedTickets { get; set; }
        public double AverageResolutionTime { get; set; }
    }
}