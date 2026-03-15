using LMS___Mini_Version.CQRS.Interns.Queries;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Interns.Queries.HandlerQuery
{
    public class GetAllInternQueryHandler : IRequestHandler<GetAllInternQuery, IEnumerable<InternDto>>
    {
        IGeneralRepository<Intern> _internRepo;
        public GetAllInternQueryHandler(IGeneralRepository<Intern> internRepo)
        {
            _internRepo = internRepo;
        }
        public async Task<IEnumerable<InternDto>> Handle(GetAllInternQuery request, CancellationToken cancellationToken)
        {
            return await _internRepo.GetTable().Select(i => new InternDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Email = i.Email,
                BirthYear = i.BirthYear,
                Status = i.Status,
                TrackId = i.TrackId,
                TrackName = i.Track.Name
            }).ToListAsync(cancellationToken);
        }
    }
}
