using LMS___Mini_Version.Domain.Entities;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Commands
{
    /// <summary>
    /// Atomic Step — stages a new Enrollment in the EF Change Tracker.
    /// Does NOT call SaveChanges. The Orchestrator commits via IUnitOfWork.
    /// Returns the tracked Enrollment entity so the Orchestrator can read the real Id after save.
    /// </summary>
    public record StageEnrollmentCommand(int InternId, int TrackId)
        : IRequest<Enrollment>;
}
