using Aesclea_Back_End_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private static List<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                Id = "1",
                PatientId = "1",
                PatientName = "John Doe",
                Department = "cardiology",
                Doctor = "Dr. Smith",
                AppointmentType = "consultation",
                DateTime = DateTime.UtcNow.AddDays(1),
                Duration = 30,
                Status = "scheduled",
                Priority = "normal",
                Reason = "Routine checkup",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Appointment
            {
                Id = "2",
                PatientId = "2",
                PatientName = "Jane Smith",
                Department = "neurology",
                Doctor = "Dr. Johnson",
                AppointmentType = "follow-up",
                DateTime = DateTime.UtcNow.AddHours(4),
                Duration = 45,
                Status = "scheduled",
                Priority = "urgent",
                Reason = "Follow-up on treatment",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        [HttpGet]
        public IActionResult GetAppointments()
        {
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public IActionResult GetAppointment(string id)
        {
            var appointment = appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            return Ok(appointment);
        }

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest("Invalid appointment data.");
            }

            appointment.Id = Guid.NewGuid().ToString();
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            
            appointments.Add(appointment);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(string id, [FromBody] Appointment updatedAppointment)
        {
            var appointment = appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            appointment.PatientName = updatedAppointment.PatientName ?? appointment.PatientName;
            appointment.Department = updatedAppointment.Department ?? appointment.Department;
            appointment.Doctor = updatedAppointment.Doctor ?? appointment.Doctor;
            appointment.AppointmentType = updatedAppointment.AppointmentType ?? appointment.AppointmentType;
            appointment.DateTime = updatedAppointment.DateTime != default ? updatedAppointment.DateTime : appointment.DateTime;
            appointment.Duration = updatedAppointment.Duration > 0 ? updatedAppointment.Duration : appointment.Duration;
            appointment.Status = updatedAppointment.Status ?? appointment.Status;
            appointment.Priority = updatedAppointment.Priority ?? appointment.Priority;
            appointment.Reason = updatedAppointment.Reason ?? appointment.Reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(string id)
        {
            var appointment = appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            appointments.Remove(appointment);
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public IActionResult GetAppointmentsByPatient(string patientId)
        {
            var patientAppointments = appointments.Where(a => a.PatientId == patientId).ToList();
            return Ok(patientAppointments);
        }

        [HttpGet("today")]
        public IActionResult GetTodayAppointments()
        {
            var today = DateTime.UtcNow.Date;
            var todayAppointments = appointments.Where(a => a.DateTime.Date == today).ToList();
            return Ok(todayAppointments);
        }
    }
}
