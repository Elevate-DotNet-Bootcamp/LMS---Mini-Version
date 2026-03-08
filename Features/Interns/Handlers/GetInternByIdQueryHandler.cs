using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Handlers;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Intern;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    /// <summary>
    /// [CQRS Assignment] Handler for GetInternByIdQuery.
    /// YOUR TASK: Find an intern by Id (include Track navigation),
    /// map to InternDetailViewModel, and return it (or null).
    /// 
    /// HINTS:
    ///   - Use _internRepository.GetTable() with .Include(i => i.Track)
    ///   - Use .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
    ///   - Map using .ToDto().ToDetailViewModel()
    ///   - Return null if not found
    /// </summary>
    public class GetInternByIdQueryHandler {

        // ╔══════════════════════════════════════════════════════════════╗
        // ║  🎯 ASSIGNMENT: Implement this handler                      ║
        // ║                                                              ║
        // ║  Find an Intern by request.Id (include Track)               ║
        // ║  Map Intern entity → InternDto → InternDetailViewModel      ║
        // ║  Return null if not found                                    ║
        // ╚══════════════════════════════════════════════════════════════╝
    }
}
#region
// : IRequestHandler<GetInternByIdQuery, InternDetailViewModel?>
//    {
//        private readonly IGeneralRepository<Intern> _internRepository;

//public GetInternByIdQueryHandler(IGeneralRepository<Intern> internRepository)
//{
//    _internRepository = internRepository;
//}

//public async Task<InternDetailViewModel?> Handle(
//    GetInternByIdQuery request, CancellationToken cancellationToken)
//{
#endregion