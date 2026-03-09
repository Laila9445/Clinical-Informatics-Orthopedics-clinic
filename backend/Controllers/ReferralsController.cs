using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CLINICSYSTEM.Data.DTOs;
using CLINICSYSTEM.Services;

namespace CLINICSYSTEM.Controllers
{
    /// <summary>
    /// Controller for managing referrals to external services (e.g., Physiotherapy)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReferralsController : ControllerBase
    {
        private readonly IReferralService _referralService;
        private readonly ILogger<ReferralsController> _logger;

        public ReferralsController(IReferralService referralService, ILogger<ReferralsController> logger)
        {
            _referralService = referralService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
        }

        /// <summary>
        /// Create a new referral to physiotherapy or other external service
        /// </summary>
        /// <param name="request">Referral details</param>
        /// <returns>Created referral information</returns>
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ProducesResponseType(typeof(ReferralDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateReferral([FromBody] CreateReferralRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var referral = await _referralService.CreateReferralAsync(request);
                if (referral == null)
                {
                    return BadRequest(new { message = "Failed to create referral" });
                }

                _logger.LogInformation("Referral created successfully by doctor {DoctorId} for patient {PatientId}",
                    request.DoctorId, request.PatientExternalId);

                return Ok(referral);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating referral");
                return StatusCode(500, new { message = "An error occurred while creating referral" });
            }
        }

        /// <summary>
        /// Get all referrals created by a specific doctor
        /// </summary>
        /// <param name="doctorId">Doctor ID</param>
        /// <param name="status">Optional: Filter by status (Pending, Sent, Accepted, InProgress, Completed, Cancelled)</param>
        /// <returns>List of referrals</returns>
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/{doctorId}")]
        [ProducesResponseType(typeof(List<ReferralDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDoctorReferrals(int doctorId, [FromQuery] string? status = null)
        {
            try
            {
                var referrals = await _referralService.GetDoctorReferralsAsync(doctorId, status);
                return Ok(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referrals for doctor {DoctorId}", doctorId);
                return StatusCode(500, new { message = "An error occurred while retrieving referrals" });
            }
        }

        /// <summary>
        /// Get all referrals for a patient (using external patient ID)
        /// </summary>
        /// <param name="patientExternalId">External patient ID from Patient Portal</param>
        /// <returns>List of referrals</returns>
        [HttpGet("patient/{patientExternalId}")]
        [ProducesResponseType(typeof(List<ReferralDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPatientReferrals(string patientExternalId)
        {
            try
            {
                var referrals = await _referralService.GetPatientReferralsAsync(patientExternalId);
                return Ok(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referrals for patient {PatientId}", patientExternalId);
                return StatusCode(500, new { message = "An error occurred while retrieving referrals" });
            }
        }

        /// <summary>
        /// Get specific referral details by ID
        /// </summary>
        /// <param name="referralId">Referral ID</param>
        /// <returns>Referral details</returns>
        [HttpGet("{referralId}")]
        [ProducesResponseType(typeof(ReferralDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetReferral(int referralId)
        {
            try
            {
                var referral = await _referralService.GetReferralByIdAsync(referralId);
                if (referral == null)
                {
                    return NotFound(new { message = "Referral not found" });
                }

                return Ok(referral);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referral {ReferralId}", referralId);
                return StatusCode(500, new { message = "An error occurred while retrieving referral" });
            }
        }

        /// <summary>
        /// Update referral status (typically called by external systems or admins)
        /// </summary>
        /// <param name="referralId">Referral ID</param>
        /// <param name="request">Status update details</param>
        /// <returns>Success message</returns>
        [HttpPut("{referralId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateReferralStatus(int referralId, [FromBody] UpdateReferralStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _referralService.UpdateReferralStatusAsync(referralId, request);
                if (!result)
                {
                    return NotFound(new { message = "Referral not found or update failed" });
                }

                _logger.LogInformation("Referral {ReferralId} status updated to: {Status}", referralId, request.Status);
                return Ok(new { message = "Referral status updated successfully", status = request.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating referral status for {ReferralId}", referralId);
                return StatusCode(500, new { message = "An error occurred while updating status" });
            }
        }

        /// <summary>
        /// Manually send referral to external physiotherapy system
        /// </summary>
        /// <param name="referralId">Referral ID</param>
        /// <returns>Success message</returns>
        [Authorize(Roles = "Doctor")]
        [HttpPost("{referralId}/send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SendToExternalSystem(int referralId)
        {
            try
            {
                var result = await _referralService.SendToExternalSystemAsync(referralId);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to send referral to external system" });
                }

                _logger.LogInformation("Referral {ReferralId} sent to external system successfully", referralId);
                return Ok(new { message = "Referral sent to external system successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending referral {ReferralId} to external system", referralId);
                return StatusCode(500, new { message = "An error occurred while sending referral" });
            }
        }
    }
}
