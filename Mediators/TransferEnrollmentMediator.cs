using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Services.Interfaces;

namespace LMS___Mini_Version.Mediators
{
    /// <summary>
    /// Action Coordinator — orchestrates the multi-step "Transfer an Enrollment" action.
    ///
    /// Workflow:
    ///   1. Fetch the existing enrollment        → IEnrollmentService.GetByIdAsync()
    ///   2. Validate new track (exists, active)   → ITrackService.GetByIdAsync()
    ///   3. Check new track capacity              → ITrackService.CheckCapacityAsync()
    ///   4. Move enrollment to new track          → IEnrollmentService.UpdateTrackAsync()    (staged)
    ///   5. Calculate fee difference + update pay → IPaymentService.UpdatePaymentAmountAsync() (staged)
    ///   6. Commit everything atomically          → IUnitOfWork.CompleteAsync()
    ///
    /// This prevents the intern from being "stuck" between two tracks
    /// if any step fails — either the full transfer succeeds or nothing changes.
    /// </summary>
    public class TransferEnrollmentMediator
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ITrackService _trackService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public TransferEnrollmentMediator(
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

        public async Task<MediatorResult> ExecuteAsync(int enrollmentId, int newTrackId)
        {
            // Step 1: Fetch the existing enrollment
            var enrollment = await _enrollmentService.GetByIdAsync(enrollmentId).ConfigureAwait(false);
            if (enrollment == null)
            {
                return MediatorResult.Fail($"Enrollment with ID {enrollmentId} was not found.");
            }

            if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                return MediatorResult.Fail("Cannot transfer a cancelled enrollment.");
            }

            if (enrollment.TrackId == newTrackId)
            {
                return MediatorResult.Fail("The intern is already enrolled in this track.");
            }

            // Step 2: Validate the new track exists and is active
            var newTrack = await _trackService.GetByIdAsync(newTrackId).ConfigureAwait(false);
            if (newTrack == null)
            {
                return MediatorResult.Fail($"Target Track with ID {newTrackId} was not found.");
            }
            if (!newTrack.IsActive)
            {
                return MediatorResult.Fail($"Target Track '{newTrack.Name}' is not currently active.");
            }

            // Step 3: Check new track capacity
            var hasCapacity = await _trackService.CheckCapacityAsync(newTrackId).ConfigureAwait(false);
            if (!hasCapacity)
            {
                return MediatorResult.Fail($"Target Track '{newTrack.Name}' has reached its maximum capacity.");
            }

            // Step 4: Move enrollment to new track (staged, NOT saved)
            var trackUpdated = await _enrollmentService
                .UpdateTrackAsync(enrollmentId, newTrackId)
                .ConfigureAwait(false);

            if (!trackUpdated)
            {
                return MediatorResult.Fail("Failed to update the enrollment's track.");
            }

            // Step 5: Calculate fee difference and update payment amount (staged, NOT saved)
            // The new track's fees become the new payment amount
            if (newTrack.Fees > 0)
            {
                await _paymentService
                    .UpdatePaymentAmountAsync(enrollmentId, newTrack.Fees)
                    .ConfigureAwait(false);
            }

            // Step 6: ATOMIC COMMIT — track change + payment adjustment saved in one transaction
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return MediatorResult.Succeed(
                $"Enrollment transferred to '{newTrack.Name}' successfully. New fees: {newTrack.Fees:C}.");
        }
    }
}
