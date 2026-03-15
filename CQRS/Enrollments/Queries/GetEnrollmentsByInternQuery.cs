using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Enrollments.Queries
{
    public record GetEnrollmentsByInternQuery(int InternId) : IRequest<IEnumerable<EnrollmentDto>>;

}
