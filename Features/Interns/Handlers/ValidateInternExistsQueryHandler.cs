using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Queries;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class ValidateInternExistsQueryHandler
        : IRequestHandler<ValidateInternExistsQuery, bool>
    {
        private readonly IGeneralRepository<Intern> _internRepository;

        public ValidateInternExistsQueryHandler(IGeneralRepository<Intern> internRepository)
        {
            _internRepository = internRepository;
        }

        public async Task<bool> Handle(
            ValidateInternExistsQuery request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository
                .GetByIdAsync(request.InternId)
                .ConfigureAwait(false);

            return intern != null;
        }
    }
}
