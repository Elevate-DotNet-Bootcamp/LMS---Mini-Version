using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class GetEnrollmentByIdQueryHandler
        : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentViewModel?>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public GetEnrollmentByIdQueryHandler(IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<EnrollmentViewModel?> Handle(
            GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Intern)
                .Include(e => e.Track)
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            return enrollment?.ToDto().ToViewModel();
        }
    }
}
