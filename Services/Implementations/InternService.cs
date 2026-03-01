using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Services.Implementations
{
    /// <summary>
    /// [SRP Fix] Injects IGeneralRepository&lt;Intern&gt; and IUnitOfWork directly.
    /// 
    /// CRUD methods (Create, Update, Delete) call CompleteAsync() internally.
    /// </summary>
    public class InternService : IInternService
    {
        private readonly IGeneralRepository<Intern> _internRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InternService(
            IGeneralRepository<Intern> internRepository,
            IUnitOfWork unitOfWork)
        {
            _internRepository = internRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<InternDto>> GetAllAsync()
        {
            var interns = await _internRepository
                .GetTable()
                .Include(i => i.Track)
                .ToListAsync()
                .ConfigureAwait(false);

            return interns.Select(i => i.ToDto());
        }

        public async Task<InternDto?> GetByIdAsync(int id)
        {
            var intern = await _internRepository
                .GetTable()
                .Include(i => i.Track)
                .FirstOrDefaultAsync(i => i.Id == id)
                .ConfigureAwait(false);

            return intern?.ToDto();
        }

        public async Task<InternDto> CreateAsync(InternDto dto)
        {
            var entity = new Intern
            {
                FullName = dto.FullName,
                Email = dto.Email,
                BirthYear = dto.BirthYear,
                Status = dto.Status,
                TrackId = dto.TrackId
            };

            _internRepository.Add(entity);

            // Save so EF populates entity.Id with the DB-generated value
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, InternDto dto)
        {
            var intern = await _internRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (intern == null) return false;

            intern.FullName = dto.FullName;
            intern.Email = dto.Email;
            intern.BirthYear = dto.BirthYear;
            intern.Status = dto.Status;
            intern.TrackId = dto.TrackId;

            _internRepository.Update(intern);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var intern = await _internRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (intern == null) return false;

            _internRepository.Delete(intern);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }
    }
}
