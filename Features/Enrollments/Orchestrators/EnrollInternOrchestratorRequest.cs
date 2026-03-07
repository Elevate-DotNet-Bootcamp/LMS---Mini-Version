using LMS___Mini_Version.Features.Common;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Orchestrators
{
    /// <summary>
    /// Orchestrator Request — replaces the old EnrollInternMediator.
    /// Dispatched through IMediator like any other request,
    /// keeping the controller constructor clean.
    /// </summary>
    public record EnrollInternOrchestratorRequest(int InternId, int TrackId)
        : IRequest<EnrollmentResultDto>;
}
