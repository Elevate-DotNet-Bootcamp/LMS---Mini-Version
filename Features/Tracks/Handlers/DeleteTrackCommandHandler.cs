using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class DeleteTrackCommandHandler
        : IRequestHandler<DeleteTrackCommand, bool>
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrackCommandHandler(
            IGeneralRepository<Track> trackRepository,
            IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteTrackCommand request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository
                .GetByIdAsync(request.Id)
                .ConfigureAwait(false);

            if (track == null) return false;

            _trackRepository.Delete(track);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }
    }
}
