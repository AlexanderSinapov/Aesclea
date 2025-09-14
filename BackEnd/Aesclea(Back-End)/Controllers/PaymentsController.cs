// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(ILogger<PaymentsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                _logger.LogInformation("Creating payment intent for amount: {Amount}", request.Amount);

                // In a real implementation, this would integrate with Stripe or another payment processor
                // For now, we'll simulate payment processing
                var paymentIntent = new PaymentIntentResponse
                {
                    Id = $"pi_{Guid.NewGuid().ToString().Replace("-", "")[..24]}",
                    ClientSecret = $"pi_{Guid.NewGuid().ToString().Replace("-", "")[..24]}_secret_{Guid.NewGuid().ToString().Replace("-", "")[..10]}",
                    Amount = request.Amount,
                    Currency = request.Currency ?? "usd",
                    Status = "requires_payment_method"
                };

                return Ok(paymentIntent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return StatusCode(500, new { message = "Internal server error during payment processing" });
            }
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                _logger.LogInformation("Confirming payment for intent: {PaymentIntentId}", request.PaymentIntentId);

                // Simulate payment confirmation
                // In a real implementation, this would confirm with the payment processor
                var confirmation = new PaymentConfirmationResponse
                {
                    PaymentIntentId = request.PaymentIntentId,
                    Status = "succeeded", // In real implementation, this would come from payment processor
                    AmountReceived = 2999, // Example amount in cents
                    Currency = "usd",
                    PaymentMethod = new PaymentMethodInfo
                    {
                        Type = "card",
                        Last4 = "4242",
                        Brand = "visa"
                    },
                    CreatedAt = DateTime.UtcNow
                };

                return Ok(confirmation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment");
                return StatusCode(500, new { message = "Internal server error during payment confirmation" });
            }
        }

        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // In a real implementation, this would fetch saved payment methods from payment processor
                var paymentMethods = new List<PaymentMethodInfo>
                {
                    new PaymentMethodInfo
                    {
                        Id = "pm_1234567890",
                        Type = "card",
                        Last4 = "4242",
                        Brand = "visa",
                        ExpiryMonth = 12,
                        ExpiryYear = 2025,
                        IsDefault = true
                    }
                };

                return Ok(paymentMethods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment methods");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("add-payment-method")]
        public async Task<IActionResult> AddPaymentMethod([FromBody] AddPaymentMethodRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                _logger.LogInformation("Adding payment method for user: {UserId}", userId);

                // In a real implementation, this would save the payment method with the payment processor
                var paymentMethod = new PaymentMethodInfo
                {
                    Id = $"pm_{Guid.NewGuid().ToString().Replace("-", "")[..16]}",
                    Type = "card",
                    Last4 = request.Last4 ?? "0000",
                    Brand = request.Brand ?? "unknown",
                    ExpiryMonth = request.ExpiryMonth,
                    ExpiryYear = request.ExpiryYear,
                    IsDefault = request.SetAsDefault
                };

                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment method");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("payment-methods/{paymentMethodId}")]
        public async Task<IActionResult> RemovePaymentMethod(string paymentMethodId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                _logger.LogInformation("Removing payment method {PaymentMethodId} for user {UserId}", paymentMethodId, userId);

                // In a real implementation, this would remove the payment method from the payment processor
                return Ok(new { message = "Payment method removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing payment method");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Simulate invoice data
                var invoices = new List<Invoice>
                {
                    new Invoice
                    {
                        Id = "inv_1",
                        Number = "INV-001",
                        Amount = 2999,
                        Currency = "usd",
                        Status = "paid",
                        DueDate = DateTime.UtcNow.AddDays(-30),
                        PaidAt = DateTime.UtcNow.AddDays(-25),
                        CreatedAt = DateTime.UtcNow.AddDays(-35)
                    }
                };

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoices");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return Math.Abs(userIdClaim.GetHashCode());
        }
    }

    // Request Models
    public class CreatePaymentIntentRequest
    {
        public int Amount { get; set; } // Amount in cents
        public string? Currency { get; set; } = "usd";
        public string? Description { get; set; }
        public string? PlanId { get; set; }
    }

    public class ConfirmPaymentRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string? PaymentMethodId { get; set; }
    }

    public class AddPaymentMethodRequest
    {
        public string? Last4 { get; set; }
        public string? Brand { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool SetAsDefault { get; set; } = false;
    }

    // Response Models
    public class PaymentIntentResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class PaymentConfirmationResponse
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AmountReceived { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentMethodInfo PaymentMethod { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentMethodInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Last4 { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
    }

    public class Invoice
    {
        public string Id { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
