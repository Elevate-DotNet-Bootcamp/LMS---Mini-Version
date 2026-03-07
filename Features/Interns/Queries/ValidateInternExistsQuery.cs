using MediatR;

namespace LMS___Mini_Version.Features.Interns.Queries
{
    /// <summary>
    /// Checks if an intern exists in the database.
    /// Used by Orchestrators for validation before enrollment.
    /// </summary>
    public record ValidateInternExistsQuery(int InternId) : IRequest<bool>;
}
