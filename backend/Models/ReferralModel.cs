using System.ComponentModel.DataAnnotations;

namespace CLINICSYSTEM.Models
{
    /// <summary>
    /// Represents a referral from a doctor to another service (e.g., Physiotherapy)
    /// </summary>
    public class ReferralModel
    {
        [Key]
        public int ReferralId { get; set; }
        
        /// <summary>
        /// External patient ID from the Patient Portal system
        /// </summary>
        [Required]
        public string PatientExternalId { get; set; } = string.Empty;
        
        /// <summary>
        /// ID of the doctor creating the referral
        /// </summary>
        [Required]
        public int DoctorId { get; set; }
        
        /// <summary>
        /// Type of referral (e.g., "Physiotherapy", "Radiology", etc.)
        /// </summary>
        [Required]
        public string ReferralType { get; set; } = string.Empty;
        
        /// <summary>
        /// Reason for referral
        /// </summary>
        [Required]
        public string Reason { get; set; } = string.Empty;
        
        /// <summary>
        /// Doctor's diagnosis
        /// </summary>
        [Required]
        public string Diagnosis { get; set; } = string.Empty;
        
        /// <summary>
        /// Recommended treatment plan
        /// </summary>
        public string RecommendedTreatment { get; set; } = string.Empty;
        
        /// <summary>
        /// Priority level: Low, Normal, High, Urgent
        /// </summary>
        public string Priority { get; set; } = "Normal";
        
        /// <summary>
        /// Referral status: Pending, Sent, Accepted, InProgress, Completed, Cancelled
        /// </summary>
        public string Status { get; set; } = "Pending";
        
        /// <summary>
        /// ID of the referral in the external system (e.g., Physiotherapy system)
        /// </summary>
        public string? ExternalReferralId { get; set; }
        
        /// <summary>
        /// URL of the external service that received the referral
        /// </summary>
        public string? ExternalServiceUrl { get; set; }
        
        /// <summary>
        /// Additional notes from the referring doctor
        /// </summary>
        public string? DoctorNotes { get; set; }
        
        /// <summary>
        /// Feedback from the external service (e.g., physiotherapist)
        /// </summary>
        public string? ExternalServiceFeedback { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public DoctorModel? Doctor { get; set; }
    }
}
