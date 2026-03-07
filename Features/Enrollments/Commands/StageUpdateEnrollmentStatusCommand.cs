using LMS___Mini_Version.Domain.Enums;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Commands
{
    /// <summary>
    /// Atomic Step — stages an enrollment status update in the Change Tracker.
    /// Does NOT call SaveChanges.
    /// </summary>
    public record StageUpdateEnrollmentStatusCommand(
        int EnrollmentId,
        EnrollmentStatus NewStatus
    ) : IRequest<bool>;
}
