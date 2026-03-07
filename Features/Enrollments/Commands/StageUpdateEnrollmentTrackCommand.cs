using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Commands
{
    /// <summary>
    /// Atomic Step — stages an enrollment track transfer in the Change Tracker.
    /// Does NOT call SaveChanges.
    /// </summary>
    public record StageUpdateEnrollmentTrackCommand(
        int EnrollmentId,
        int NewTrackId
    ) : IRequest<bool>;
}
