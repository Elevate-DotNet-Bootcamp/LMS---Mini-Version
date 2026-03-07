using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Common;
using LMS___Mini_Version.Features.Enrollments.Commands;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Features.Payments.Commands;
using LMS___Mini_Version.Features.Tracks.Queries;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Orchestrators
{
    /// <summary>
    /// Orchestrator Handler — coordinates the "Transfer an Enrollment" workflow.
    ///
    /// Workflow:
    ///   1. Fetch enrollment                → GetEnrollmentByIdQuery
    ///   2. Validate (not cancelled, different track)
    ///   3. Validate new track (exists/active) → GetTrackByIdQuery
    ///   4. Check new track capacity        → CheckTrackCapacityQuery
    ///   5. Stage track transfer            → StageUpdateEnrollmentTrackCommand   (staged)
    ///   6. Stage payment adjustment        → StageUpdatePaymentAmountCommand     (staged)
    ///   7. COMMIT atomically               → IUnitOfWork.CompleteAsync()
    /// </summary>
    public class TransferEnrollmentOrchestratorHandler
        : IRequestHandler<TransferEnrollmentOrchestratorRequest, CommandResult>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public TransferEnrollmentOrchestratorHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResult> Handle(
            TransferEnrollmentOrchestratorRequest request, CancellationToken cancellationToken)
        {
            // Step 1: Fetch the existing enrollment
            var enrollment = await _mediator
                .Send(new GetEnrollmentByIdQuery(request.EnrollmentId), cancellationToken)
                .ConfigureAwait(false);

            if (enrollment == null)
            {
                return CommandResult.Fail(
                    $"Enrollment with ID {request.EnrollmentId} was not found.");
            }

            // Step 2: Validate
            if (enrollment.Status == EnrollmentStatus.Cancelled.ToString())
            {
                return CommandResult.Fail("Cannot transfer a cancelled enrollment.");
            }

            // Step 3: Validate the new track exists and is active
            var newTrack = await _mediator
                .Send(new GetTrackByIdQuery(request.NewTrackId), cancellationToken)
                .ConfigureAwait(false);

            if (newTrack == null)
            {
                return CommandResult.Fail(
                    $"Target Track with ID {request.NewTrackId} was not found.");
            }

            if (!newTrack.IsActive)
            {
                return CommandResult.Fail(
                    $"Target Track '{newTrack.Name}' is not currently active.");
            }

            // Step 4: Check new track capacity
            var hasCapacity = await _mediator
                .Send(new CheckTrackCapacityQuery(request.NewTrackId), cancellationToken)
                .ConfigureAwait(false);

            if (!hasCapacity)
            {
                return CommandResult.Fail(
                    $"Target Track '{newTrack.Name}' has reached its maximum capacity.");
            }

            // Step 5: Stage track transfer (NOT saved)
            var trackUpdated = await _mediator
                .Send(new StageUpdateEnrollmentTrackCommand(
                    request.EnrollmentId, request.NewTrackId), cancellationToken)
                .ConfigureAwait(false);

            if (!trackUpdated)
            {
                return CommandResult.Fail("Failed to update the enrollment's track.");
            }

            // Step 6: Stage payment adjustment (NOT saved)
            if (newTrack.Fees > 0)
            {
                await _mediator
                    .Send(new StageUpdatePaymentAmountCommand(
                        request.EnrollmentId, newTrack.Fees), cancellationToken)
                    .ConfigureAwait(false);
            }

            // Step 7: ATOMIC COMMIT — track change + payment adjustment
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return CommandResult.Succeed(
                $"Enrollment transferred to '{newTrack.Name}' successfully. New fees: {newTrack.Fees:C}.");
        }
    }
}
