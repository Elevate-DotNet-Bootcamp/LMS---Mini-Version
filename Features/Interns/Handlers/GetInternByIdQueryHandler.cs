using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Intern;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class GetInternByIdQueryHandler
        : IRequestHandler<GetInternByIdQuery, InternDetailViewModel?>
    {
        private readonly IGeneralRepository<Intern> _internRepository;

        public GetInternByIdQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<InternDetailViewModel?> Handle(
            GetInternByIdQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository
                .GetTable()
                .Include(i => i.Track)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            return intern?.ToDto().ToDetailViewModel();
        }
    }
}
