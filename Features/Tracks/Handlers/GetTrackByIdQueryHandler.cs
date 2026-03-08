using Azure.Core;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Features.Tracks.Handlers;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    /// <summary>
    /// [CQRS Assignment] Handler for GetTrackByIdQuery.
    /// YOUR TASK: Find a track by its Id, map it to TrackDetailViewModel, and return it.
    /// 
    /// HINTS:
    ///   - Use _trackRepository.GetByIdAsync(request.Id)
    ///   - Map using .ToDto().ToDetailViewModel()
    ///   - Return null if not found
    /// </summary>
    public class GetTrackByIdQueryHandler : IRequestHandler<GetTrackByIdQuery, TrackDto>

    {
        private IGeneralRepository<Track> _trackRepository;
        
        public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }
        public async Task<TrackDto> Handle(GetTrackByIdQuery request, CancellationToken cancellationToken)
        {
            var track = await _trackRepository.GetByIdAsync(request.Id);
            if (track == null) return null;

            var trackDTO = new TrackDto
            {
                Id = track.Id,
                Name = track.Name,
                Fees = track.Fees,
                IsActive = track.IsActive,
                MaxCapacity = track.MaxCapacity,
                CurrentEnrollmentCount = track.Enrollments?.Count ?? 0
            };
            return trackDTO;
        }
    }
}
#region 
//answer

//: IRequestHandler<GetTrackByIdQuery, TrackDetailViewModel?>

//private readonly IGeneralRepository<Track> _trackRepository;

//public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepository)
//{
//    _trackRepository = trackRepository;
//}

//var track = await _trackRepository.GetByIdAsync(request.Id).ConfigureAwait(false);
//if (track == null) return null;



//return trackDetailViewModel;
#endregion