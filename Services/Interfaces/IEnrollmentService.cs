using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.DTOs;

namespace LMS___Mini_Version.Services.Interfaces
{
    /// <summary>
    /// [Trap 5 Fix] Service layer for Enrollment — single-entity Steps only.
    /// </summary>
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentDto>> GetAllAsync();
        Task<EnrollmentDto?> GetByIdAsync(int id);

        /// <summary>
        /// Creates the enrollment entity in memory (staged in Change Tracker).
        /// Returns the tracked entity so the Mediator can read the real Id after SaveChanges.
        /// Does NOT call SaveChanges — that's the Mediator's job via UoW.
        /// </summary>
        Task<Enrollment> CreateEnrollmentAsync(CreateEnrollmentDto dto);

        Task<IEnumerable<EnrollmentDto>> GetByInternAsync(int internId);

        /// <summary>
        /// Atomic Step: Updates the enrollment status (e.g., Active → Cancelled).
        /// Staged only — no SaveChanges.
        /// </summary>
        Task<bool> UpdateStatusAsync(int enrollmentId, EnrollmentStatus newStatus);

        /// <summary>
        /// Atomic Step: Moves an enrollment to a different track.
        /// Staged only — no SaveChanges.
        /// </summary>
        Task<bool> UpdateTrackAsync(int enrollmentId, int newTrackId);
    }
}
