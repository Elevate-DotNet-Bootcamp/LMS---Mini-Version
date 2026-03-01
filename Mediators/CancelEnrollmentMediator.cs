using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Services.Interfaces;

namespace LMS___Mini_Version.Mediators
{
    /// <summary>
    /// Action Coordinator — orchestrates the multi-step "Cancel an Enrollment" action.
    ///
    /// Workflow:
    ///   1. Fetch the enrollment             → IEnrollmentService.GetByIdAsync()
    ///   2. Change status to Cancelled       → IEnrollmentService.UpdateStatusAsync()     (staged)
    ///   3. Process refund for payment       → IPaymentService.RefundPaymentAsync()       (staged)
    ///   4. Commit everything atomically     → IUnitOfWork.CompleteAsync()
    ///
    /// By cancelling enrollment AND refunding payment in one commit,
    /// we guarantee: either BOTH happen, or NEITHER happens (no partial state).
    /// </summary>
    public class CancelEnrollmentMediator
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ITrackService _trackService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollmentMediator(
            IEnrollmentService enrollmentService,
            ITrackService trackService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
        {
            _enrollmentService = enrollmentService;
            _trackService = trackService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MediatorResult> ExecuteAsync(int enrollmentId)
        {
            // Step 1: Fetch the enrollment
            var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId).ConfigureAwait(false);
            if (enrollment == null)
            {
                return MediatorResult.Fail($"Enrollment with ID {enrollmentId} was not found.");
            }

            if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                return MediatorResult.Fail("This enrollment is already cancelled.");
            }

            // Step 2: Change enrollment status to Cancelled (staged, NOT saved)
            var statusUpdated = await _enrollmentService
                .UpdateStatusAsync(enrollmentId, EnrollmentStatus.Cancelled)
                .ConfigureAwait(false);

            if (!statusUpdated)
            {
                return MediatorResult.Fail("Failed to update enrollment status.");
            }

            // Step 3: Process refund for the associated payment (staged, NOT saved)
            // If no payment exists (free track), this simply returns false — which is fine.
            await _paymentService.RefundPaymentAsync(enrollmentId).ConfigureAwait(false);

            // Step 4: ATOMIC COMMIT — cancellation + refund saved in one transaction
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return MediatorResult.Succeed("Enrollment cancelled and payment refunded successfully.");
        }
    }
}
