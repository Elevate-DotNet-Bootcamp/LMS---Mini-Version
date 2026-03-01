using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Services.Interfaces
{
    /// <summary>
    /// [Trap 5 Fix] Service layer for Payment — single-entity Steps only.
    /// </summary>
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto?> GetByEnrollmentAsync(int enrollmentId);

        /// <summary>
        /// Creates a payment record in memory (staged in Change Tracker).
        /// Does NOT call SaveChanges — committed atomically by UoW.
        /// </summary>
        Task<PaymentDto> CreatePaymentAsync(PaymentDto dto);

        /// <summary>
        /// Atomic Step: Marks the payment as Refunded.
        /// Staged only — no SaveChanges.
        /// </summary>
        Task<bool> RefundPaymentAsync(int enrollmentId);

        /// <summary>
        /// Atomic Step: Updates the payment amount (e.g., when transferring to a track with different fees).
        /// Staged only — no SaveChanges.
        /// </summary>
        Task<bool> UpdatePaymentAmountAsync(int enrollmentId, decimal newAmount);
    }
}
