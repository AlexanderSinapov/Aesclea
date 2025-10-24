// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.EntityFrameworkCore;
using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Services
{
    public interface ISupportService
    {
        Task<TicketResponse> CreateTicketAsync(string userId, CreateTicketRequest request);
        Task<TicketResponse> GetTicketByIdAsync(string ticketId, string userId, bool isAgent = false);
        Task<TicketResponse> GetUserTicketsAsync(string userId, int page = 1, int pageSize = 10);
        Task<TicketResponse> GetAllTicketsAsync(int page = 1, int pageSize = 10, TicketStatus? status = null, string? assignedAgentId = null);
        Task<TicketResponse> UpdateTicketAsync(string ticketId, UpdateTicketRequest request, string updatedBy);
        Task<TicketMessageResponse> SendMessageAsync(string ticketId, string senderId, SendMessageRequest request, bool isFromAgent = false);
        Task<TicketMessageResponse> GetTicketMessagesAsync(string ticketId, string userId, bool isAgent = false);
        Task<TicketStatsResponse> GetTicketStatsAsync(string? agentId = null);
        Task<bool> IsUserSupportAgentAsync(string userId);
        Task<IEnumerable<User>> GetSupportAgentsAsync();
    }

    public class SupportService : ISupportService
    {
        private readonly AescleaDbContext _context;

        public SupportService(AescleaDbContext context)
        {
            _context = context;
        }

        public async Task<TicketResponse> CreateTicketAsync(string userId, CreateTicketRequest request)
        {
            try
            {
                var ticket = new SupportTicket
                {
                    UserId = userId,
                    Subject = request.Subject,
                    Description = request.Description,
                    Priority = request.Priority,
                    Category = request.Category,
                    Status = TicketStatus.Open
                };

                _context.SupportTickets.Add(ticket);
                await _context.SaveChangesAsync();

                // Create initial message with the description
                var initialMessage = new TicketMessage
                {
                    TicketId = ticket.Id,
                    SenderId = userId,
                    Content = request.Description,
                    IsFromAgent = false
                };

                _context.TicketMessages.Add(initialMessage);
                await _context.SaveChangesAsync();

                // Load the ticket with user information
                var createdTicket = await _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedAgent)
                    .Include(t => t.Messages)
                    .FirstOrDefaultAsync(t => t.Id == ticket.Id);

                return new TicketResponse
                {
                    Success = true,
                    Message = "Support ticket created successfully",
                    Ticket = createdTicket
                };
            }
            catch (Exception ex)
            {
                return new TicketResponse
                {
                    Success = false,
                    Message = $"Error creating ticket: {ex.Message}"
                };
            }
        }

        public async Task<TicketResponse> GetTicketByIdAsync(string ticketId, string userId, bool isAgent = false)
        {
            try
            {
                var query = _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedAgent)
                    .Include(t => t.Messages.OrderBy(m => m.CreatedAt))
                        .ThenInclude(m => m.Sender)
                    .Include(t => t.Attachments)
                    .AsQueryable();

                if (!isAgent)
                {
                    query = query.Where(t => t.UserId == userId);
                }

                var ticket = await query.FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket == null)
                {
                    return new TicketResponse
                    {
                        Success = false,
                        Message = "Ticket not found or access denied"
                    };
                }

                return new TicketResponse
                {
                    Success = true,
                    Message = "Ticket retrieved successfully",
                    Ticket = ticket
                };
            }
            catch (Exception ex)
            {
                return new TicketResponse
                {
                    Success = false,
                    Message = $"Error retrieving ticket: {ex.Message}"
                };
            }
        }

        public async Task<TicketResponse> GetUserTicketsAsync(string userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedAgent)
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt);

                var tickets = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new TicketResponse
                {
                    Success = true,
                    Message = "Tickets retrieved successfully",
                    Tickets = tickets
                };
            }
            catch (Exception ex)
            {
                return new TicketResponse
                {
                    Success = false,
                    Message = $"Error retrieving tickets: {ex.Message}"
                };
            }
        }

        public async Task<TicketResponse> GetAllTicketsAsync(int page = 1, int pageSize = 10, TicketStatus? status = null, string? assignedAgentId = null)
        {
            try
            {
                var query = _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedAgent)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(t => t.Status == status.Value);
                }

                if (!string.IsNullOrEmpty(assignedAgentId))
                {
                    query = query.Where(t => t.AssignedAgentId == assignedAgentId);
                }

                var tickets = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new TicketResponse
                {
                    Success = true,
                    Message = "Tickets retrieved successfully",
                    Tickets = tickets
                };
            }
            catch (Exception ex)
            {
                return new TicketResponse
                {
                    Success = false,
                    Message = $"Error retrieving tickets: {ex.Message}"
                };
            }
        }

        public async Task<TicketResponse> UpdateTicketAsync(string ticketId, UpdateTicketRequest request, string updatedBy)
        {
            try
            {
                var ticket = await _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedAgent)
                    .FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket == null)
                {
                    return new TicketResponse
                    {
                        Success = false,
                        Message = "Ticket not found"
                    };
                }

                bool hasChanges = false;

                if (request.Status.HasValue && ticket.Status != request.Status.Value)
                {
                    ticket.Status = request.Status.Value;
                    hasChanges = true;

                    if (request.Status.Value == TicketStatus.Resolved || request.Status.Value == TicketStatus.Closed)
                    {
                        ticket.ResolvedAt = DateTime.UtcNow;
                    }
                }

                if (request.Priority.HasValue && ticket.Priority != request.Priority.Value)
                {
                    ticket.Priority = request.Priority.Value;
                    hasChanges = true;
                }

                if (request.Category.HasValue && ticket.Category != request.Category.Value)
                {
                    ticket.Category = request.Category.Value;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(request.AssignedAgentId) && ticket.AssignedAgentId != request.AssignedAgentId)
                {
                    ticket.AssignedAgentId = request.AssignedAgentId;
                    hasChanges = true;
                }

                if (!string.IsNullOrEmpty(request.ResolutionNote))
                {
                    ticket.ResolutionNote = request.ResolutionNote;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    ticket.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return new TicketResponse
                {
                    Success = true,
                    Message = "Ticket updated successfully",
                    Ticket = ticket
                };
            }
            catch (Exception ex)
            {
                return new TicketResponse
                {
                    Success = false,
                    Message = $"Error updating ticket: {ex.Message}"
                };
            }
        }

        public async Task<TicketMessageResponse> SendMessageAsync(string ticketId, string senderId, SendMessageRequest request, bool isFromAgent = false)
        {
            try
            {
                var ticket = await _context.SupportTickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return new TicketMessageResponse
                    {
                        Success = false,
                        Message = "Ticket not found"
                    };
                }

                var message = new TicketMessage
                {
                    TicketId = ticketId,
                    SenderId = senderId,
                    Content = request.Content,
                    IsFromAgent = isFromAgent
                };

                _context.TicketMessages.Add(message);

                // Update ticket status if message is from agent
                if (isFromAgent && ticket.Status == TicketStatus.Waiting)
                {
                    ticket.Status = TicketStatus.InProgress;
                }
                else if (!isFromAgent && ticket.Status == TicketStatus.InProgress)
                {
                    ticket.Status = TicketStatus.Waiting;
                }

                ticket.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Load the message with sender information
                var createdMessage = await _context.TicketMessages
                    .Include(m => m.Sender)
                    .FirstOrDefaultAsync(m => m.Id == message.Id);

                return new TicketMessageResponse
                {
                    Success = true,
                    Message = "Message sent successfully",
                    TicketMessage = createdMessage
                };
            }
            catch (Exception ex)
            {
                return new TicketMessageResponse
                {
                    Success = false,
                    Message = $"Error sending message: {ex.Message}"
                };
            }
        }

        public async Task<TicketMessageResponse> GetTicketMessagesAsync(string ticketId, string userId, bool isAgent = false)
        {
            try
            {
                var ticket = await _context.SupportTickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return new TicketMessageResponse
                    {
                        Success = false,
                        Message = "Ticket not found"
                    };
                }

                // Check if user has access to this ticket
                if (!isAgent && ticket.UserId != userId)
                {
                    return new TicketMessageResponse
                    {
                        Success = false,
                        Message = "Access denied"
                    };
                }

                var messages = await _context.TicketMessages
                    .Include(m => m.Sender)
                    .Where(m => m.TicketId == ticketId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return new TicketMessageResponse
                {
                    Success = true,
                    Message = "Messages retrieved successfully",
                    Messages = messages
                };
            }
            catch (Exception ex)
            {
                return new TicketMessageResponse
                {
                    Success = false,
                    Message = $"Error retrieving messages: {ex.Message}"
                };
            }
        }

        public async Task<TicketStatsResponse> GetTicketStatsAsync(string? agentId = null)
        {
            try
            {
                var query = _context.SupportTickets.AsQueryable();

                if (!string.IsNullOrEmpty(agentId))
                {
                    query = query.Where(t => t.AssignedAgentId == agentId);
                }

                var stats = new TicketStatsResponse
                {
                    TotalTickets = await query.CountAsync(),
                    OpenTickets = await query.CountAsync(t => t.Status == TicketStatus.Open),
                    InProgressTickets = await query.CountAsync(t => t.Status == TicketStatus.InProgress),
                    ResolvedTickets = await query.CountAsync(t => t.Status == TicketStatus.Resolved),
                    ClosedTickets = await query.CountAsync(t => t.Status == TicketStatus.Closed)
                };

                if (!string.IsNullOrEmpty(agentId))
                {
                    stats.MyAssignedTickets = await _context.SupportTickets
                        .CountAsync(t => t.AssignedAgentId == agentId && 
                                   (t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress || t.Status == TicketStatus.Waiting));
                }

                // Calculate average resolution time
                var resolvedTickets = await query
                    .Where(t => t.Status == TicketStatus.Resolved && t.ResolvedAt.HasValue)
                    .Select(t => new { t.CreatedAt, t.ResolvedAt })
                    .ToListAsync();

                if (resolvedTickets.Any())
                {
                    var totalMinutes = resolvedTickets
                        .Sum(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalMinutes);
                    stats.AverageResolutionTime = totalMinutes / resolvedTickets.Count;
                }

                return stats;
            }
            catch (Exception)
            {
                return new TicketStatsResponse();
            }
        }

        public async Task<bool> IsUserSupportAgentAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                return user?.Role?.ToLower() == "supportagent" || user?.Role?.ToLower() == "admin";
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetSupportAgentsAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.Role.ToLower() == "supportagent" || u.Role.ToLower() == "admin")
                    .ToListAsync();
            }
            catch
            {
                return new List<User>();
            }
        }
    }
}