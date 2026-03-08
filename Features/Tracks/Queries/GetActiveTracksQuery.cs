using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    /// <summary>
    /// [CQRS Assignment] Query to retrieve only the active tracks.
    /// </summary>
    public record GetActiveTracksQuery : IRequest<IEnumerable<TrackSummaryViewModel>>;


    #region
   // : IRequest<IEnumerable<TrackSummaryViewModel>>
    #endregion
}
