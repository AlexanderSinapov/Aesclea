using Aesclea_Back_End_.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private static List<Patient> patients = new List<Patient>();

        [HttpGet]
        public IActionResult GetPatients()
        {
            return Ok(patients);
        }

        [HttpPost]
        public IActionResult AddPatient([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Invalid patient data.");
            }
            patients.Add(patient);
            return CreatedAtAction(nameof(GetPatients), new { id = patient.Id }, patient);
        }
    }
}
