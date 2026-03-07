using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    /// <summary>
    /// Standalone command handler — creates the Track and calls CompleteAsync.
    /// Not an orchestrated step, so it saves directly.
    /// </summary>
    public class CreateTrackCommandHandler
        : IRequestHandler<CreateTrackCommand, TrackSummaryViewModel>
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTrackCommandHandler(
            IGeneralRepository<Track> trackRepository,
            IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TrackSummaryViewModel> Handle(
            CreateTrackCommand request, CancellationToken cancellationToken)
        {
            var entity = new Track
            {
                Name = request.Name,
                Fees = request.Fees,
                IsActive = request.IsActive,
                MaxCapacity = request.MaxCapacity
            };

            _trackRepository.Add(entity);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return entity.ToDto().ToSummaryViewModel();
        }
    }
}
