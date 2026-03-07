using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Common;
using LMS___Mini_Version.Features.Enrollments.Commands;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Features.Payments.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Orchestrators
{
    /// <summary>
    /// Orchestrator Handler — coordinates the "Cancel an Enrollment" workflow.
    ///
    /// Workflow:
    ///   1. Fetch enrollment               → GetEnrollmentByIdQuery
    ///   2. Validate not already cancelled
    ///   3. Stage status → Cancelled       → StageUpdateEnrollmentStatusCommand  (staged)
    ///   4. Stage payment refund           → StageRefundPaymentCommand            (staged)
    ///   5. COMMIT atomically              → IUnitOfWork.CompleteAsync()
    /// </summary>
    public class CancelEnrollmentOrchestratorHandler
        : IRequestHandler<CancelEnrollmentOrchestratorRequest, CommandResult>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollmentOrchestratorHandler(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommandResult> Handle(
            CancelEnrollmentOrchestratorRequest request, CancellationToken cancellationToken)
        {
            // Step 1: Fetch the enrollment
            var enrollment = await _mediator
                .Send(new GetEnrollmentByIdQuery(request.EnrollmentId), cancellationToken)
                .ConfigureAwait(false);

            if (enrollment == null)
            {
                return CommandResult.Fail(
                    $"Enrollment with ID {request.EnrollmentId} was not found.");
            }

            // Step 2: Validate not already cancelled
            if (enrollment.Status == EnrollmentStatus.Cancelled.ToString())
            {
                return CommandResult.Fail("This enrollment is already cancelled.");
            }

            // Step 3: Stage status change to Cancelled (NOT saved)
            var statusUpdated = await _mediator
                .Send(new StageUpdateEnrollmentStatusCommand(
                    request.EnrollmentId, EnrollmentStatus.Cancelled), cancellationToken)
                .ConfigureAwait(false);

            if (!statusUpdated)
            {
                return CommandResult.Fail("Failed to update enrollment status.");
            }

            // Step 4: Stage payment refund (NOT saved)
            await _mediator
                .Send(new StageRefundPaymentCommand(request.EnrollmentId), cancellationToken)
                .ConfigureAwait(false);

            // Step 5: ATOMIC COMMIT — cancellation + refund in one transaction
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return CommandResult.Succeed(
                "Enrollment cancelled and payment refunded successfully.");
        }
    }
}
