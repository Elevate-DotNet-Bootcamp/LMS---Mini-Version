using LMS___Mini_Version.Features.Common;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Orchestrators
{
    /// <summary>
    /// Orchestrator Request — replaces the old TransferEnrollmentMediator.
    /// </summary>
    public record TransferEnrollmentOrchestratorRequest(int EnrollmentId, int NewTrackId)
        : IRequest<CommandResult>;
}
