using Microsoft.AspNetCore.Mvc;

namespace Aesclea_Back_End_.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosisController : ControllerBase
    {
        [HttpPost("suggest")]
        public IActionResult SuggestDiagnosis([FromBody] string symptoms)
        {
            var suggestedDiagnosis = "Common Cold";
            return Ok(new { Diagnosis = suggestedDiagnosis });
        }   
    }
}
