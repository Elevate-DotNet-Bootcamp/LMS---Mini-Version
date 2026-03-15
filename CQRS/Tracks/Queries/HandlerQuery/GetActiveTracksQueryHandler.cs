using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Tracks.Queries.HandlerQuery
{
    public class GetActiveTracksQueryHandler : IRequestHandler<GetActiveTracksQuery, IEnumerable<TrackDto>>
    {
        IGeneralRepository<Track> _trackRepo;
        public GetActiveTracksQueryHandler(IGeneralRepository<Track> trackRepo)
        {
            _trackRepo = trackRepo;
        }
        public async Task<IEnumerable<TrackDto>> Handle(GetActiveTracksQuery request, CancellationToken cancellationToken)
        {
            return await _trackRepo.GetTable().Where(t => t.IsActive)
                                   .Include(t => t.Enrollments)
                                   .Select(t => t.ToDto())
                                   .ToListAsync(cancellationToken);
        }
    }
}
