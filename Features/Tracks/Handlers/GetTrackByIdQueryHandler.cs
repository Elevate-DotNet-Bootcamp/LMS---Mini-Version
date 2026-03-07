using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class GetTrackByIdQueryHandler
        : IRequestHandler<GetTrackByIdQuery, TrackDetailViewModel?>
    {
        private readonly IGeneralRepository<Track> _trackRepository;

        public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<TrackDetailViewModel?> Handle(
            GetTrackByIdQuery request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository
                .GetByIdAsync(request.Id)
                .ConfigureAwait(false);

            return track?.ToDto().ToDetailViewModel();
        }
    }
}
