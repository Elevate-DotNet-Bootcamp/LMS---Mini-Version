using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Commands
{
    /// <summary>
    /// Standalone command — updates an existing Track and saves immediately.
    /// Returns true if the track was found and updated.
    /// </summary>
    public record UpdateTrackCommand(
        int Id,
        string Name,
        decimal Fees,
        bool IsActive,
        int MaxCapacity
    ) : IRequest<bool>;
}
