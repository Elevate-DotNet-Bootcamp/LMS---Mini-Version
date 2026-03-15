using LMS___Mini_Version.CQRS.Interns.Queries;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Interns.Queries.HandlerQuery
{
    public class GetInternByIdQueryHandler : IRequestHandler<GetInternByIdQuery, InternDto>
    {
        IGeneralRepository<Intern> _internRepo;
        public GetInternByIdQueryHandler(IGeneralRepository<Intern> internRepo)
        {
            _internRepo = internRepo;
        }
        public async Task<InternDto> Handle(GetInternByIdQuery request, CancellationToken cancellationToken)
        {
            return await _internRepo.GetTable().Where(i => i.Id == request.id)
                .Select(i => new InternDto
                {
                    Id = i.Id,
                    FullName = i.FullName,
                    Email = i.Email,
                    BirthYear = i.BirthYear,
                    Status = i.Status,
                    TrackId = i.TrackId,
                    TrackName = i.Track.Name
                }).FirstOrDefaultAsync(cancellationToken);

        }
    }
}
