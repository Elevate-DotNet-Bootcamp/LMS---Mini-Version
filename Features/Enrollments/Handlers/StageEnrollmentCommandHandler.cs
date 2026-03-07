using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    /// <summary>
    /// Atomic Step — stages a new Enrollment in the Change Tracker.
    /// Does NOT call SaveChanges. Returns the tracked entity
    /// so the Orchestrator can read the real Id after CompleteAsync().
    /// </summary>
    public class StageEnrollmentCommandHandler
        : IRequestHandler<StageEnrollmentCommand, Enrollment>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public StageEnrollmentCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public Task<Enrollment> Handle(
            StageEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var entity = new Enrollment
            {
                InternId = request.InternId,
                TrackId = request.TrackId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending
            };

            _enrollmentRepository.Add(entity);

            // Return the tracked entity — NOT saved yet
            return Task.FromResult(entity);
        }
    }
}
