using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    /// <summary>
    /// Atomic Step — updates enrollment status in the Change Tracker.
    /// Does NOT call SaveChanges.
    /// </summary>
    public class StageUpdateEnrollmentStatusCommandHandler
        : IRequestHandler<StageUpdateEnrollmentStatusCommand, bool>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public StageUpdateEnrollmentStatusCommandHandler(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> Handle(
            StageUpdateEnrollmentStatusCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository
                .GetByIdAsync(request.EnrollmentId)
                .ConfigureAwait(false);

            if (enrollment == null) return false;

            enrollment.Status = request.NewStatus;
            _enrollmentRepository.Update(enrollment);
            return true;
        }
    }
}
