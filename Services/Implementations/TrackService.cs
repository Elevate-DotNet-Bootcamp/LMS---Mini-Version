using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Services.Implementations
{
    /// <summary>
    /// [SRP Fix] Injects IGeneralRepository&lt;Track&gt;, IGeneralRepository&lt;Enrollment&gt;, and IUnitOfWork directly.
    /// Repositories handle data access; UoW handles transaction commits.
    /// 
    /// [Trap 3 Fix] All methods are fully async.
    /// [Trap 4 Fix] Uses GetTable() (IQueryable) for DB-side queries.
    /// [Trap 5 Fix] Contains only single-entity "Steps" — no cross-entity orchestration.
    /// 
    /// CRUD methods (Create, Update, Delete) call CompleteAsync() internally and return final results.
    /// Mediator-coordinated methods (CheckCapacityAsync) only read — no save needed.
    /// </summary>
    public class TrackService : ITrackService
    {
        private readonly IGeneralRepository<Track> _trackRepository;
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TrackService(
            IGeneralRepository<Track> trackRepository,
            IGeneralRepository<Enrollment> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _trackRepository = trackRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrackDto>> GetAllAsync()
        {
            var tracks = await _trackRepository
                .GetTable()
                .Include(t => t.Enrollments)
                .ToListAsync()
                .ConfigureAwait(false);

            return tracks.Select(t => t.ToDto());
        }

        public async Task<TrackDto?> GetByIdAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id).ConfigureAwait(false);
            return track?.ToDto();
        }

        public async Task<TrackDto> CreateAsync(TrackDto dto)
        {
            var entity = new Track
            {
                Name = dto.Name,
                Fees = dto.Fees,
                IsActive = dto.IsActive,
                MaxCapacity = dto.MaxCapacity
            };

            _trackRepository.Add(entity);

            // Save so EF populates entity.Id with the DB-generated value
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            // Now entity.Id has the real value — return accurate DTO
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, TrackDto dto)
        {
            var track = await _trackRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (track == null) return false;

            track.Name = dto.Name;
            track.Fees = dto.Fees;
            track.IsActive = dto.IsActive;
            track.MaxCapacity = dto.MaxCapacity;

            _trackRepository.Update(track);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (track == null) return false;

            _trackRepository.Delete(track);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Business Step: checks if the track's active enrollment count is below MaxCapacity.
        /// Read-only — no save needed.
        /// </summary>
        public async Task<bool> CheckCapacityAsync(int trackId)
        {
            var activeCount = await _enrollmentRepository
                .GetTable()
                .CountAsync(e => e.TrackId == trackId
                              && e.Status != Domain.Enums.EnrollmentStatus.Cancelled)
                .ConfigureAwait(false);

            var track = await _trackRepository.GetByIdAsync(trackId).ConfigureAwait(false);
            if (track == null) return false;

            return activeCount < track.MaxCapacity;
        }
    }
}
