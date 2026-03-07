using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    /// <summary>
    /// Checks if a track has room for another enrollment.
    /// Returns true if activeEnrollmentCount &lt; MaxCapacity.
    /// </summary>
    public record CheckTrackCapacityQuery(int TrackId) : IRequest<bool>;
}
