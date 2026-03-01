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
    /// [SRP Fix] Injects IGeneralRepository&lt;Payment&gt; and IUnitOfWork directly.
    /// 
    /// Mediator-coordinated methods (CreatePaymentAsync, RefundPaymentAsync, UpdatePaymentAmountAsync)
    /// only STAGE changes — the Mediator calls UoW.CompleteAsync() at the end for atomicity.
    /// 
    /// Read methods are unaffected.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IGeneralRepository<Payment> paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository
                .GetTable()
                .Include(p => p.Enrollment)
                .ToListAsync()
                .ConfigureAwait(false);

            return payments.Select(p => p.ToDto());
        }

        public async Task<PaymentDto?> GetByEnrollmentAsync(int enrollmentId)
        {
            var payment = await _paymentRepository
                .GetTable()
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId)
                .ConfigureAwait(false);

            return payment?.ToDto();
        }

        /// <summary>
        /// Stages a new Payment entity in the Change Tracker.
        /// Does NOT call SaveChanges — the Mediator commits via UoW for atomicity.
        /// </summary>
        public async Task<PaymentDto> CreatePaymentAsync(PaymentDto dto)
        {
            var entity = new Payment
            {
                EnrollmentId = dto.EnrollmentId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                Method = dto.Method,
                Status = PaymentStatus.Pending
            };

            _paymentRepository.Add(entity);
            return entity.ToDto();
        }

        /// <summary>
        /// Atomic Step: marks the payment associated with an enrollment as Refunded.
        /// Staged only — no SaveChanges. The Mediator commits via UoW.
        /// </summary>
        public async Task<bool> RefundPaymentAsync(int enrollmentId)
        {
            var payment = await _paymentRepository
                .GetTable()
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId)
                .ConfigureAwait(false);

            if (payment == null) return false;

            payment.Status = PaymentStatus.Refunded;
            _paymentRepository.Update(payment);
            return true;
        }

        /// <summary>
        /// Atomic Step: updates the payment amount for a given enrollment.
        /// Used when transferring to a track with different fees.
        /// Staged only — no SaveChanges. The Mediator commits via UoW.
        /// </summary>
        public async Task<bool> UpdatePaymentAmountAsync(int enrollmentId, decimal newAmount)
        {
            var payment = await _paymentRepository
                .GetTable()
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId)
                .ConfigureAwait(false);

            if (payment == null) return false;

            payment.Amount = newAmount;
            payment.Status = PaymentStatus.Pending; // Reset to pending after fee adjustment
            _paymentRepository.Update(payment);
            return true;
        }
    }
}
