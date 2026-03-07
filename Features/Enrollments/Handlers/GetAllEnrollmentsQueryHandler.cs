using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class GetAllEnrollmentsQueryHandler
        : IRequestHandler<GetAllEnrollmentsQuery, IEnumerable<EnrollmentViewModel>>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public GetAllEnrollmentsQueryHandler(IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<EnrollmentViewModel>> Handle(
            GetAllEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Intern)
                .Include(e => e.Track)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return enrollments.Select(e => e.ToDto().ToViewModel());
        }
    }
}
