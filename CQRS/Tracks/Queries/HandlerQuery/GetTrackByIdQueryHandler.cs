using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using MediatR;
using System.Diagnostics.Eventing.Reader;

namespace LMS___Mini_Version.CQRS.Tracks.Queries.HandlerQuery
{
    public class GetTrackByIdQueryHandler : IRequestHandler<GetTrackByIdQuery, TrackDto>
    {
        IGeneralRepository<Track> _trackRepo;

        public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepo)
        {
            _trackRepo = trackRepo;
        }

        public async Task<TrackDto> Handle(GetTrackByIdQuery request, CancellationToken cancellationToken)
        {
            var Track = await _trackRepo.GetByIdAsync(request.id);

            if (Track == null)
                return null;

            var TrackDto = new TrackDto
            {
                Id = Track.Id,
                Name = Track.Name,
                Fees = Track.Fees,
                IsActive = Track.IsActive,
                MaxCapacity = Track.MaxCapacity,
                CurrentEnrollmentCount = Track.Enrollments.Count
            };

            return TrackDto;

        }
    }
}
