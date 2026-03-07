using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    public class UpdateTrackCommandHandler
        : IRequestHandler<UpdateTrackCommand, bool>
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrackCommandHandler(
            IGeneralRepository<Track> trackRepository,
            IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateTrackCommand request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository
                .GetByIdAsync(request.Id)
                .ConfigureAwait(false);

            if (track == null) return false;

            track.Name = request.Name;
            track.Fees = request.Fees;
            track.IsActive = request.IsActive;
            track.MaxCapacity = request.MaxCapacity;

            _trackRepository.Update(track);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }
    }
}
