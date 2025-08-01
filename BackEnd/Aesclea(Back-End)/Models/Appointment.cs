namespace Aesclea_Back_End_.Models
{
    public class Appointment
    {
        public string Id { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Doctor { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int Duration { get; set; } // Duration in minutes
        public string Status { get; set; } = string.Empty; // scheduled, completed, cancelled, rescheduled
        public string Priority { get; set; } = string.Empty; // normal, urgent, emergency
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
