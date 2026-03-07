using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetAllTracksQueryHandler
        : IRequestHandler<GetAllTracksQuery, IEnumerable<TrackSummaryViewModel>>
    {
        private readonly IGeneralRepository<Track> _trackRepository;

        public GetAllTracksQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<IEnumerable<TrackSummaryViewModel>> Handle(
            GetAllTracksQuery request, CancellationToken cancellationToken)
        {
            var tracks = await _trackRepository
                .GetTable()
                .Include(t => t.Enrollments)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return tracks.Select(t => t.ToDto().ToSummaryViewModel());
        }
    }
}
