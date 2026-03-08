using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class UpdateInternCommandHandler
        : IRequestHandler<UpdateInternCommand, bool>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInternCommandHandler(
            IGeneralRepository<Intern> internRepository,
            IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            UpdateInternCommand request, CancellationToken cancellationToken)
        {
            var intern = await _internRepository
                .GetByIdAsync(request.Id)
                .ConfigureAwait(false);

            if (intern == null) return false;

            intern.FullName = request.FullName;
            intern.Email = request.Email;
            intern.BirthYear = request.BirthYear;
            intern.Status = request.Status;
            intern.TrackId = request.TrackId;

            _internRepository.Update(intern);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }
    }
}
