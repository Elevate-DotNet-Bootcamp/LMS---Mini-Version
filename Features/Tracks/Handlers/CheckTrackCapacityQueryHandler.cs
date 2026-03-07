using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    /// <summary>
    /// Checks if a track has room for another enrollment.
    /// Uses the Track navigation property to count active enrollments (one repo rule).
    /// </summary>
    public class CheckTrackCapacityQueryHandler
        : IRequestHandler<CheckTrackCapacityQuery, bool>
    {
        private readonly IGeneralRepository<Track> _trackRepository;

        public CheckTrackCapacityQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public async Task<bool> Handle(
            CheckTrackCapacityQuery request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository
                .GetTable()
                .Include(t => t.Enrollments)
                .FirstOrDefaultAsync(t => t.Id == request.TrackId, cancellationToken)
                .ConfigureAwait(false);

            if (track == null) return false;

            var activeCount = track.Enrollments
                .Count(e => e.Status != EnrollmentStatus.Cancelled);

            return activeCount < track.MaxCapacity;
        }
    }
}
