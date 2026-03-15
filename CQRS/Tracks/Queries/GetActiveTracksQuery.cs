using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Tracks.Queries
{
    public record GetActiveTracksQuery : IRequest<IEnumerable<TrackDto>>;
}
