using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    public record GetAllTracksQuery : IRequest<IEnumerable<TrackSummaryViewModel>>;
}
