using Aesclea_Back_End_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private static List<Patient> patients = new List<Patient>
        {
            new Patient
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1-555-0101",
                DateOfBirth = new DateTime(1985, 5, 15),
                Gender = "Male",
                MedicalHistory = "No significant medical history",
                Department = "cardiology",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Patient
            {
                Id = "2",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "+1-555-0102",
                DateOfBirth = new DateTime(1992, 8, 22),
                Gender = "Female",
                MedicalHistory = "Allergic to penicillin",
                Department = "neurology",
                Status = "active",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Patient
            {
                Id = "3",
                FirstName = "Michael",
                LastName = "Johnson",
                Email = "michael.johnson@example.com",
                Phone = "+1-555-0103",
                DateOfBirth = new DateTime(1978, 12, 3),
                Gender = "Male",
                MedicalHistory = "Hypertension, diabetes type 2",
                Department = "cardiology",
                Status = "critical",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddHours(-6)
            }
        };

        [HttpGet]
        public IActionResult GetPatients()
        {
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public IActionResult GetPatient(string id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            return Ok(patient);
        }

        [HttpPost]
        public IActionResult AddPatient([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Invalid patient data.");
            }

            patient.Id = Guid.NewGuid().ToString();
            patient.CreatedAt = DateTime.UtcNow;
            patient.UpdatedAt = DateTime.UtcNow;
            
            patients.Add(patient);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePatient(string id, [FromBody] Patient updatedPatient)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            patient.FirstName = updatedPatient.FirstName ?? patient.FirstName;
            patient.LastName = updatedPatient.LastName ?? patient.LastName;
            patient.Email = updatedPatient.Email ?? patient.Email;
            patient.Phone = updatedPatient.Phone ?? patient.Phone;
            patient.DateOfBirth = updatedPatient.DateOfBirth ?? patient.DateOfBirth;
            patient.Gender = updatedPatient.Gender ?? patient.Gender;
            patient.MedicalHistory = updatedPatient.MedicalHistory ?? patient.MedicalHistory;
            patient.Department = updatedPatient.Department ?? patient.Department;
            patient.Status = updatedPatient.Status ?? patient.Status;
            patient.UpdatedAt = DateTime.UtcNow;

            return Ok(patient);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePatient(string id)
        {
            var patient = patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            patients.Remove(patient);
            return NoContent();
        }

        [HttpGet("department/{department}")]
        public IActionResult GetPatientsByDepartment(string department)
        {
            var departmentPatients = patients.Where(p => 
                string.Equals(p.Department, department, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(departmentPatients);
        }

        [HttpGet("search")]
        public IActionResult SearchPatients([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(patients);
            }

            var searchResults = patients.Where(p =>
                p.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Phone.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(searchResults);
        }
    }
}
