using LMS___Mini_Version.ViewModels.Intern;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Commands
{
    public record CreateInternCommand(
        string FullName,
        string Email,
        int BirthYear,
        string Status,
        int TrackId
    ) : IRequest<InternSummaryViewModel>;
}
