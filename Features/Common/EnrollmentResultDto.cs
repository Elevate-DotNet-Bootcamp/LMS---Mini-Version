using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Features.Common
{
    /// <summary>
    /// Result wrapper for the enrollment orchestrator.
    /// Moved from the old Mediators namespace to Features.Common.
    /// </summary>
    public class EnrollmentResultDto
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public EnrollmentDto? Enrollment { get; set; }
        public PaymentDto? Payment { get; set; }

        public static EnrollmentResultDto Fail(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };

        public static EnrollmentResultDto Succeed(EnrollmentDto enrollment, PaymentDto? payment) => new()
        {
            IsSuccess = true,
            Enrollment = enrollment,
            Payment = payment
        };
    }
}
