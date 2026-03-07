using LMS___Mini_Version.Features.Common;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Orchestrators
{
    /// <summary>
    /// Orchestrator Request — replaces the old CancelEnrollmentMediator.
    /// </summary>
    public record CancelEnrollmentOrchestratorRequest(int EnrollmentId)
        : IRequest<CommandResult>;
}
