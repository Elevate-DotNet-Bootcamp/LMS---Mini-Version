using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Commands
{
    /// <summary>
    /// Standalone command — deletes a Track and saves immediately.
    /// Returns true if the track was found and deleted.
    /// </summary>
    public record DeleteTrackCommand(int Id) : IRequest<bool>;
}
