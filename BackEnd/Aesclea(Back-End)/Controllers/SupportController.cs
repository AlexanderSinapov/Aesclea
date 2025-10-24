// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Aesclea_Back_End_.Models;
using Aesclea_Back_End_.Services;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        private async Task<bool> IsCurrentUserAgent()
        {
            var userId = GetCurrentUserId();
            return await _supportService.IsUserSupportAgentAsync(userId);
        }

        [HttpPost("tickets")]
        public async Task<ActionResult<TicketResponse>> CreateTicket([FromBody] CreateTicketRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(new TicketResponse
                    {
                        Success = false,
                        Message = $"Validation failed: {string.Join(", ", errors)}"
                    });
                }

                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new TicketResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var response = await _supportService.CreateTicketAsync(userId, request);
                
                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("tickets")]
        public async Task<ActionResult<TicketResponse>> GetTickets(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new TicketResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var response = await _supportService.GetUserTicketsAsync(userId, page, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("tickets/{ticketId}")]
        public async Task<ActionResult<TicketResponse>> GetTicket(string ticketId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new TicketResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var isAgent = await IsCurrentUserAgent();
                var response = await _supportService.GetTicketByIdAsync(ticketId, userId, isAgent);
                
                if (!response.Success)
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPost("tickets/{ticketId}/messages")]
        public async Task<ActionResult<TicketMessageResponse>> SendMessage(
            string ticketId, 
            [FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new TicketMessageResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var isAgent = await IsCurrentUserAgent();
                var response = await _supportService.SendMessageAsync(ticketId, userId, request, isAgent);
                
                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketMessageResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("tickets/{ticketId}/messages")]
        public async Task<ActionResult<TicketMessageResponse>> GetTicketMessages(string ticketId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new TicketMessageResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var isAgent = await IsCurrentUserAgent();
                var response = await _supportService.GetTicketMessagesAsync(ticketId, userId, isAgent);
                
                if (!response.Success)
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketMessageResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        // Support Agent Only Endpoints
        [HttpGet("agent/tickets")]
        public async Task<ActionResult<TicketResponse>> GetAllTickets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] TicketStatus? status = null,
            [FromQuery] string? assignedAgentId = null)
        {
            try
            {
                var isAgent = await IsCurrentUserAgent();
                if (!isAgent)
                {
                    return Forbid("Access denied. Support agent role required.");
                }

                var response = await _supportService.GetAllTicketsAsync(page, pageSize, status, assignedAgentId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPut("agent/tickets/{ticketId}")]
        public async Task<ActionResult<TicketResponse>> UpdateTicket(
            string ticketId,
            [FromBody] UpdateTicketRequest request)
        {
            try
            {
                var isAgent = await IsCurrentUserAgent();
                if (!isAgent)
                {
                    return Forbid("Access denied. Support agent role required.");
                }

                var userId = GetCurrentUserId();
                var response = await _supportService.UpdateTicketAsync(ticketId, request, userId);
                
                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new TicketResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("agent/stats")]
        public async Task<ActionResult<TicketStatsResponse>> GetTicketStats([FromQuery] string? agentId = null)
        {
            try
            {
                var isAgent = await IsCurrentUserAgent();
                if (!isAgent)
                {
                    return Forbid("Access denied. Support agent role required.");
                }

                // If no agentId specified, use current user's ID for personal stats
                if (string.IsNullOrEmpty(agentId))
                {
                    agentId = GetCurrentUserId();
                }

                var response = await _supportService.GetTicketStatsAsync(agentId);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new TicketStatsResponse());
            }
        }

        [HttpGet("agent/agents")]
        public async Task<ActionResult<IEnumerable<User>>> GetSupportAgents()
        {
            try
            {
                var isAgent = await IsCurrentUserAgent();
                if (!isAgent)
                {
                    return Forbid("Access denied. Support agent role required.");
                }

                var agents = await _supportService.GetSupportAgentsAsync();
                return Ok(agents);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<User>());
            }
        }
    }
}