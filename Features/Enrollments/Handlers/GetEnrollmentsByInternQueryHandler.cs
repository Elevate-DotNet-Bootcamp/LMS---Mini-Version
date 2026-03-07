using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Enrollments.Handlers
{
    public class GetEnrollmentsByInternQueryHandler
        : IRequestHandler<GetEnrollmentsByInternQuery, IEnumerable<EnrollmentViewModel>>
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;

        public GetEnrollmentsByInternQueryHandler(
            IGeneralRepository<Enrollment> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<EnrollmentViewModel>> Handle(
            GetEnrollmentsByInternQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Track)
                .Include(e => e.Intern)
                .Where(e => e.InternId == request.InternId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return enrollments.Select(e => e.ToDto().ToViewModel());
        }
    }
}
