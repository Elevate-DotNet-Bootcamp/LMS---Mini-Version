using MediatR;

namespace LMS___Mini_Version.Features.Interns.Commands
{
    public record UpdateInternCommand(
        int Id,
        string FullName,
        string Email,
        int BirthYear,
        string Status,
        int TrackId
    ) : IRequest<bool>;
}
