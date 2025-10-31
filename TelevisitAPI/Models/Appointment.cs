using System;
using System.ComponentModel.DataAnnotations;

namespace TelevisitAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Call Initiated";  // Default status

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // 🔹 Optional fields for Twilio integration
        public string? CallSid { get; set; }        // Twilio Call SID
        public string? UserResponse { get; set; }   // "1" (Yes), "2" (No), or null
    }
}
