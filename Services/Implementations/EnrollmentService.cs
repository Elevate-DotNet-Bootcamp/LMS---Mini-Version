using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Services.Implementations
{
    /// <summary>
    /// [SRP Fix] Injects IGeneralRepository&lt;Enrollment&gt; and IUnitOfWork directly.
    /// 
    /// Mediator-coordinated methods (CreateEnrollmentAsync, UpdateStatusAsync, UpdateTrackAsync)
    /// only STAGE changes — the Mediator calls UoW.CompleteAsync() at the end for atomicity.
    /// 
    /// Read methods are unaffected.
    /// </summary>
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGeneralRepository<Enrollment> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(
            IGeneralRepository<Enrollment> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllAsync()
        {
            var enrollments = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Intern)
                .Include(e => e.Track)
                .ToListAsync()
                .ConfigureAwait(false);

            return enrollments.Select(e => e.ToDto());
        }

        public async Task<EnrollmentDto?> GetByIdAsync(int id)
        {
            var enrollment = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Intern)
                .Include(e => e.Track)
                .FirstOrDefaultAsync(e => e.Id == id)
                .ConfigureAwait(false);

            return enrollment?.ToDto();
        }

        /// <summary>
        /// Stages a new Enrollment entity in the Change Tracker.
        /// Returns the tracked entity so the Mediator can read the real Id after SaveChanges.
        /// Does NOT call SaveChanges — the Mediator will call UoW.CompleteAsync()
        /// after all steps (enrollment + payment) are staged for atomicity.
        /// </summary>
        public async Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentDto dto)
        {
            var entity = new Enrollment
            {
                InternId = dto.InternId,
                TrackId = dto.TrackId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending
            };

            _enrollmentRepository.Add(entity);
            return entity; // Return the tracked entity, not a DTO
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByInternAsync(int internId)
        {
            var enrollments = await _enrollmentRepository
                .GetTable()
                .Include(e => e.Track)
                .Include(e => e.Intern)
                .Where(e => e.InternId == internId)
                .ToListAsync()
                .ConfigureAwait(false);

            return enrollments.Select(e => e.ToDto());
        }

        /// <summary>
        /// Atomic Step: updates the enrollment status in the Change Tracker.
        /// No SaveChanges — the Mediator commits via UoW.
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int enrollmentId, EnrollmentStatus newStatus)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId).ConfigureAwait(false);
            if (enrollment == null) return false;

            enrollment.Status = newStatus;
            _enrollmentRepository.Update(enrollment);
            return true;
        }

        /// <summary>
        /// Atomic Step: moves the enrollment to a new track in the Change Tracker.
        /// No SaveChanges — the Mediator commits via UoW.
        /// </summary>
        public async Task<bool> UpdateTrackAsync(int enrollmentId, int newTrackId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId).ConfigureAwait(false);
            if (enrollment == null) return false;

            enrollment.TrackId = newTrackId;
            _enrollmentRepository.Update(enrollment);
            return true;
        }
    }
}
