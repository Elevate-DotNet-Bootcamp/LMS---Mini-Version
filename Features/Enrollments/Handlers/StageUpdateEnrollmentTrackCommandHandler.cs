using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    /// <summary>
    /// Atomic Step — moves enrollment to a new track in the Change Tracker.
    /// Does NOT call SaveChanges.
    /// </summary>
    public class StageUpdateEnrollmentTrackCommandHandler
        : IRequestHandler<StageUpdateEnrollmentTrackCommand, bool>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public StageUpdateEnrollmentTrackCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> Handle(
            StageUpdateEnrollmentTrackCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository
                .GetByIdAsync(request.EnrollmentId)
                .ConfigureAwait(false);

            if (enrollment == null) return false;

            enrollment.TrackId = request.NewTrackId;
            _enrollmentRepository.Update(enrollment);
            return true;
        }
    }
}
