using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Interns.Queries
{
    public record GetInternByIdQuery(int id) : IRequest<InternDto?>;
}
