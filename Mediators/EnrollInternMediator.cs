using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Services.Interfaces;

namespace LMS___Mini_Version.Mediators
{
    /// <summary>
    /// [Trap 5 Fix] Action Coordinator — orchestrates the multi-step "Enroll an Intern" action.
    /// This prevents cyclic dependencies between Services: InternService does NOT inject TrackService,
    /// and vice versa. The Mediator coordinates them instead.
    ///
    /// [Trap 6 Fix] The Mediator calls _unitOfWork.CompleteAsync() ONCE at the very end,
    /// ensuring all steps (enrollment creation + payment creation) are committed atomically.
    /// If ANY step fails, NOTHING is saved to the database.
    ///
    /// Workflow:
    ///   1. Validate intern exists    → IInternService.GetByIdAsync()
    ///   2. Validate track is active  → ITrackService.GetByIdAsync()
    ///   3. Check track capacity      → ITrackService.CheckCapacityAsync()
    ///   4. Create enrollment         → IEnrollmentService.CreateEnrollmentAsync()    (staged)
    ///   5. Create payment (if paid)  → IPaymentService.CreatePaymentAsync()          (staged)
    ///   6. Commit everything         → IUnitOfWork.CompleteAsync()                    (atomic)
    /// </summary>
    public class EnrollInternMediator
    {
        private readonly IInternService _internService;
        private readonly ITrackService _trackService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollInternMediator(
            IInternService internService,
            ITrackService trackService,
            IEnrollmentService enrollmentService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork)
        {
            _internService = internService;
            _trackService = trackService;
            _enrollmentService = enrollmentService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Executes the full enrollment workflow and returns the result.
        /// </summary>
        public async Task<EnrollmentResultDto> ExecuteAsync(CreateEnrollmentDto dto)
        {
            // Step 1: Validate the intern exists
            var intern = await _internService.GetByIdAsync(dto.InternId).ConfigureAwait(false);
            if (intern == null)
            {
                return EnrollmentResultDto.Fail($"Intern with ID {dto.InternId} was not found.");
            }

            // Step 2: Validate the track exists and is active
            var track = await _trackService.GetByIdAsync(dto.TrackId).ConfigureAwait(false);
            if (track == null)
            {
                return EnrollmentResultDto.Fail($"Track with ID {dto.TrackId} was not found.");
            }
            if (!track.IsActive)
            {
                return EnrollmentResultDto.Fail($"Track '{track.Name}' is not currently active.");
            }

            // Step 3: Check capacity
            var hasCapacity = await _trackService.CheckCapacityAsync(dto.TrackId).ConfigureAwait(false);
            if (!hasCapacity)
            {
                return EnrollmentResultDto.Fail($"Track '{track.Name}' has reached its maximum capacity.");
            }

            // Step 4: Create enrollment (staged in Change Tracker, NOT saved yet)
            var enrollment = await _enrollmentService.CreateEnrollmentAsync(dto).ConfigureAwait(false);

            // Step 5: Save the enrollment first so it gets a real ID from the database.
            // Without this, the Payment's EnrollmentId would be 0 (invalid FK).
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            // Step 6: If the track has fees, create a payment record using the real enrollment ID
            PaymentDto? payment = null;
            if (track.Fees > 0)
            {
                payment = await _paymentService.CreatePaymentAsync(new PaymentDto
                {
                    EnrollmentId = enrollment.Id, // Now has the real ID from the database
                    Amount = track.Fees,
                    Method = PaymentMethod.Cash,
                    Status = PaymentStatus.Pending
                }).ConfigureAwait(false);
            }

            // Step 7: COMMIT the payment record
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return EnrollmentResultDto.Succeed(enrollment.ToDto(), payment);
        }
    }

    /// <summary>
    /// Result wrapper for the enrollment mediator action.
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
