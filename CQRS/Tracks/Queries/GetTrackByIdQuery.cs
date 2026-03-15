using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Tracks.Queries
{
    public record GetTrackByIdQuery (int id) : IRequest<TrackDto?>;

    //public class GetTrackByIdQueryHandler : IRequestHandler<GetTrackByIdQuery, TrackDto?>
    //{
    //    IGeneralRepository<Track> _trackRepo;
    //    public GetTrackByIdQueryHandler(IGeneralRepository<Track> trackRepo)
    //    {
    //        _trackRepo = trackRepo;
    //    }
    //    public async Task<TrackDto?> Handle(GetTrackByIdQuery request, CancellationToken cancellationToken)
    //    {

    //        var track = await _trackRepo.GetTable()
    //            .Where(t => t.Id == request.id)
    //            .Select(t => new TrackDto
    //            {
    //                Id = request.id,
    //                Name = t.Name,
    //                Fees = t.Fees,
    //                IsActive = t.IsActive,
    //                MaxCapacity = t.MaxCapacity,
    //                CurrentEnrollmentCount = t.Enrollments.Count()
    //            }).FirstOrDefaultAsync(cancellationToken);

    //        if (track == null)
    //        {
    //            return null;
    //        }
    //        return track;

    //    }
    //}

}
