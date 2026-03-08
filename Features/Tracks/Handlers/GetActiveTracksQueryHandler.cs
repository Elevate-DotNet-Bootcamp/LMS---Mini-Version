using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Handlers
{
    /// <summary>
    /// [CQRS Assignment] Handler for GetActiveTracksQuery.
    /// YOUR TASK: Query the database for tracks where IsActive == true,
    /// map them to TrackSummaryViewModel, and return the list.
    /// 
    /// HINTS:
    ///   - Use _trackRepository.GetTable() to get IQueryable
    ///   - Filter with .Where(t => t.IsActive)
    ///   - Use .ToListAsync(cancellationToken)
    ///   - Map using .ToDto().ToSummaryViewModel()
    /// </summary>
    public class GetActiveTracksQueryHandler : IRequestHandler<GetActiveTracksQuery, IEnumerable<TrackSummaryViewModel>>
    {
        // ╔══════════════════════════════════════════════════════════════╗
        // ║  🎯 ASSIGNMENT: Implement this handler                      ║
        // ║                                                              ║
        // ║  Return all tracks where IsActive == true                    ║
        // ║  Map each Track entity → TrackDto → TrackSummaryViewModel   ║
        // ╚══════════════════════════════════════════════════════════════╝
        private readonly IGeneralRepository<Track> _trackRepository;
        public GetActiveTracksQueryHandler(IGeneralRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        #region
        //        : IRequestHandler<GetActiveTracksQuery, IEnumerable<TrackSummaryViewModel>>
        //{
        //    private readonly IGeneralRepository<Track> _trackRepository;

        //public GetActiveTracksQueryHandler(IGeneralRepository<Track> trackRepository)
        //{
        //    _trackRepository = trackRepository;
        //}
        #endregion
        public Task<IEnumerable<TrackSummaryViewModel>> Handle(GetActiveTracksQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Implement GetActiveTracksQueryHandler to return active tracks as TrackSummaryViewModel");

        }
    }
}
