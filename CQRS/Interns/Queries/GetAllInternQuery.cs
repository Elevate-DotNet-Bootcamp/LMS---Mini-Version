using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Interns.Queries
{
    public record GetAllInternQuery : IRequest<IEnumerable<InternDto>>;
}
