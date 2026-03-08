using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;

namespace LMS___Mini_Version.Features.Tracks.Queries
{
    public record GetTrackByIdQuery (int Id) : IRequest<TrackDto>;


}

#region 

// : IRequest<TrackDetailViewModel?>
//public class GetTrackByIdQuery : IRequest<TrackDetailViewModel?>
//{
//    public int Id { get; }

//    public GetTrackByIdQuery(int id)
//    {
//        Id = id;
//    }
//}
//}   
#endregion