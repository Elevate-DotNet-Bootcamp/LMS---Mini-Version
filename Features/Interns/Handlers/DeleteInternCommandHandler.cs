using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class DeleteInternCommandHandler
        : IRequestHandler<DeleteInternCommand, bool>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteInternCommandHandler(
            IGeneralRepository<Intern> internRepository,
            IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteInternCommand request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository
                .GetByIdAsync(request.Id)
                .ConfigureAwait(false);

            if (intern == null) return false;

            _internRepository.Delete(intern);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }
    }
}
