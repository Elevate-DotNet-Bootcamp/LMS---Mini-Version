using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Enrollments.Queries.QueryHandler
{
    public class GetEnrollmentsByInternQueryHandler : IRequestHandler<GetEnrollmentsByInternQuery, IEnumerable<EnrollmentDto>>
    {
        IGeneralRepository<Enrollment> _enrollmentRepo;
        public GetEnrollmentsByInternQueryHandler(IGeneralRepository<Enrollment> enrollmentRepo)
        {
            _enrollmentRepo = enrollmentRepo;
        }
        public async Task<IEnumerable<EnrollmentDto>> Handle(GetEnrollmentsByInternQuery request, CancellationToken cancellationToken)
        {
           return await _enrollmentRepo.GetTable()
                .Where(e => e.InternId == request.InternId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    InternId = e.InternId,
                    InternName = e.Intern.FullName,
                    TrackId = e.TrackId,
                    TrackName = e.Track.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .ToListAsync(cancellationToken);
        }
    }
}
