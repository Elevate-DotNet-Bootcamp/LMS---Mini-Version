using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Commands
{
    /// <summary>
    /// Standalone command — creates a new Track and saves immediately.
    /// Returns the created track as a summary ViewModel.
    /// </summary>
    public record CreateTrackCommand(
        string Name,
        decimal Fees,
        bool IsActive,
        int MaxCapacity
    ) : IRequest<TrackSummaryViewModel>;
}
