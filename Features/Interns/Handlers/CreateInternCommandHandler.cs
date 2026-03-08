using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Interns.Commands;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Intern;
using MediatR;

namespace LMS___Mini_Version.Features.Interns.Handlers
{
    public class CreateInternCommandHandler
        : IRequestHandler<CreateInternCommand, InternSummaryViewModel>
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInternCommandHandler(
            IGeneralRepository<Intern> internRepository,
            IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InternSummaryViewModel> Handle(
            CreateInternCommand request, CancellationToken cancellationToken)
        {
            var entity = new Intern
            {
                FullName = request.FullName,
                Email = request.Email,
                BirthYear = request.BirthYear,
                Status = request.Status,
                TrackId = request.TrackId
            };

            _internRepository.Add(entity);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return entity.ToDto().ToSummaryViewModel();
        }
    }
}
