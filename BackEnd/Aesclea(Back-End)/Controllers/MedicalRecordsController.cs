// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

using Aesclea_Back_End_.Data;
using Aesclea_Back_End_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aesclea_Back_End_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly AescleaDbContext _context;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(AescleaDbContext context, ILogger<MedicalRecordsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Referrals (Направления)
        [HttpPost("referrals")]
        public async Task<IActionResult> CreateReferral([FromBody] CreateReferralRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var user = await _context.Users.FindAsync(userId);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                var patient = await _context.Patients.FindAsync(request.PatientId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                var referral = new Referral
                {
                    AppointmentId = request.AppointmentId,
                    PatientId = request.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    DoctorId = userId!,
                    DoctorName = $"{user.FirstName} {user.LastName}",
                    Specialty = request.Specialty,
                    Reason = request.Reason,
                    Notes = request.Notes,
                    ExpiryDate = request.ExpiryDate ?? DateTime.UtcNow.AddMonths(3)
                };

                _context.Referrals.Add(referral);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, referral });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating referral");
                return StatusCode(500, new { message = "Failed to create referral" });
            }
        }

        [HttpGet("referrals/appointment/{appointmentId}")]
        public async Task<IActionResult> GetReferralsByAppointment(string appointmentId)
        {
            try
            {
                var referrals = await _context.Referrals
                    .Where(r => r.AppointmentId == appointmentId)
                    .ToListAsync();

                return Ok(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching referrals");
                return StatusCode(500, new { message = "Failed to fetch referrals" });
            }
        }

        // Diagnoses (Диагнози)
        [HttpPost("diagnoses")]
        public async Task<IActionResult> CreateDiagnosis([FromBody] CreateDiagnosisRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                
                var diagnosis = new Diagnosis
                {
                    AppointmentId = request.AppointmentId,
                    PatientId = request.PatientId,
                    DoctorId = userId!,
                    ICD10Code = request.ICD10Code,
                    DiagnosisName = request.DiagnosisName,
                    DiagnosisType = request.DiagnosisType,
                    ClinicalFindings = request.ClinicalFindings,
                    Notes = request.Notes
                };

                _context.Diagnoses.Add(diagnosis);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, diagnosis });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diagnosis");
                return StatusCode(500, new { message = "Failed to create diagnosis" });
            }
        }

        [HttpGet("diagnoses/appointment/{appointmentId}")]
        public async Task<IActionResult> GetDiagnosesByAppointment(string appointmentId)
        {
            try
            {
                var diagnoses = await _context.Diagnoses
                    .Where(d => d.AppointmentId == appointmentId)
                    .ToListAsync();

                return Ok(diagnoses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching diagnoses");
                return StatusCode(500, new { message = "Failed to fetch diagnoses" });
            }
        }

        // Prescriptions (Лекарства)
        [HttpPost("prescriptions")]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                
                var prescription = new Prescription
                {
                    AppointmentId = request.AppointmentId,
                    PatientId = request.PatientId,
                    DoctorId = userId!,
                    MedicationName = request.MedicationName,
                    Dosage = request.Dosage,
                    Frequency = request.Frequency,
                    Route = request.Route,
                    Duration = request.Duration,
                    Instructions = request.Instructions,
                    Quantity = request.Quantity,
                    StartDate = request.StartDate ?? DateTime.UtcNow,
                    EndDate = (request.StartDate ?? DateTime.UtcNow).AddDays(request.Duration)
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, prescription });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, new { message = "Failed to create prescription" });
            }
        }

        [HttpGet("prescriptions/appointment/{appointmentId}")]
        public async Task<IActionResult> GetPrescriptionsByAppointment(string appointmentId)
        {
            try
            {
                var prescriptions = await _context.Prescriptions
                    .Where(p => p.AppointmentId == appointmentId)
                    .ToListAsync();

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching prescriptions");
                return StatusCode(500, new { message = "Failed to fetch prescriptions" });
            }
        }

        // Outpatient Record (Амбулаторен лист)
        [HttpPost("outpatient-records")]
        public async Task<IActionResult> CreateOutpatientRecord([FromBody] CreateOutpatientRecordRequest request)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var user = await _context.Users.FindAsync(userId);
                var patient = await _context.Patients.FindAsync(request.PatientId);
                
                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                var appointment = await _context.Appointments.FindAsync(request.AppointmentId);

                var record = new OutpatientRecord
                {
                    AppointmentId = request.AppointmentId,
                    PatientId = request.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    PatientEGN = request.PatientEGN,
                    DoctorId = userId!,
                    DoctorName = $"{user!.FirstName} {user.LastName}",
                    DoctorSpecialty = user.Department,
                    VisitDate = appointment?.DateTime ?? DateTime.UtcNow,
                    ChiefComplaint = request.ChiefComplaint,
                    MedicalHistory = request.MedicalHistory,
                    PhysicalExamination = request.PhysicalExamination,
                    VitalSigns = request.VitalSigns,
                    TreatmentPlan = request.TreatmentPlan,
                    Notes = request.Notes
                };

                _context.OutpatientRecords.Add(record);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, record });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating outpatient record");
                return StatusCode(500, new { message = "Failed to create outpatient record" });
            }
        }

        [HttpGet("outpatient-records/{id}")]
        public async Task<IActionResult> GetOutpatientRecord(string id)
        {
            try
            {
                var record = await _context.OutpatientRecords.FindAsync(id);
                
                if (record == null)
                {
                    return NotFound(new { message = "Record not found" });
                }

                // Get related data
                var diagnoses = await _context.Diagnoses
                    .Where(d => record.DiagnosisIds.Contains(d.Id))
                    .ToListAsync();

                var prescriptions = await _context.Prescriptions
                    .Where(p => record.PrescriptionIds.Contains(p.Id))
                    .ToListAsync();

                var referrals = await _context.Referrals
                    .Where(r => record.ReferralIds.Contains(r.Id))
                    .ToListAsync();

                return Ok(new
                {
                    record,
                    diagnoses,
                    prescriptions,
                    referrals
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching outpatient record");
                return StatusCode(500, new { message = "Failed to fetch outpatient record" });
            }
        }

        [HttpGet("outpatient-records/appointment/{appointmentId}")]
        public async Task<IActionResult> GetOutpatientRecordByAppointment(string appointmentId)
        {
            try
            {
                var record = await _context.OutpatientRecords
                    .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);

                if (record == null)
                {
                    return NotFound(new { message = "Record not found" });
                }

                // Get all related medical data for this appointment
                var diagnoses = await _context.Diagnoses
                    .Where(d => d.AppointmentId == appointmentId)
                    .ToListAsync();

                var prescriptions = await _context.Prescriptions
                    .Where(p => p.AppointmentId == appointmentId)
                    .ToListAsync();

                var referrals = await _context.Referrals
                    .Where(r => r.AppointmentId == appointmentId)
                    .ToListAsync();

                return Ok(new
                {
                    record,
                    diagnoses,
                    prescriptions,
                    referrals
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching outpatient record");
                return StatusCode(500, new { message = "Failed to fetch outpatient record" });
            }
        }

        [HttpPut("outpatient-records/{id}")]
        public async Task<IActionResult> UpdateOutpatientRecord(string id, [FromBody] CreateOutpatientRecordRequest request)
        {
            try
            {
                var record = await _context.OutpatientRecords.FindAsync(id);
                
                if (record == null)
                {
                    return NotFound(new { message = "Record not found" });
                }

                record.ChiefComplaint = request.ChiefComplaint;
                record.MedicalHistory = request.MedicalHistory;
                record.PhysicalExamination = request.PhysicalExamination;
                record.VitalSigns = request.VitalSigns;
                record.TreatmentPlan = request.TreatmentPlan;
                record.Notes = request.Notes;
                record.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, record });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating outpatient record");
                return StatusCode(500, new { message = "Failed to update outpatient record" });
            }
        }
    }
}
