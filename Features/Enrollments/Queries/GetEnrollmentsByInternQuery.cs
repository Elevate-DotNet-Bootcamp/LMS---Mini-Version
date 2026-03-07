using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Queries
{
    public record GetEnrollmentsByInternQuery(int InternId)
        : IRequest<IEnumerable<EnrollmentViewModel>>;
}
