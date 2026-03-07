using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;

namespace LMS___Mini_Version.Features.Enrollments.Queries
{
    public record GetAllEnrollmentsQuery : IRequest<IEnumerable<EnrollmentViewModel>>;
}
