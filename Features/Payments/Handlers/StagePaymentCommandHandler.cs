using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Payments.Commands;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    /// <summary>
    /// Atomic Step — stages a new Payment in the Change Tracker.
    /// Does NOT call SaveChanges. Returns the tracked entity.
    /// </summary>
    public class StagePaymentCommandHandler
        : IRequestHandler<StagePaymentCommand, Payment>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public StagePaymentCommandHandler(IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public Task<Payment> Handle(
            StagePaymentCommand request, CancellationToken cancellationToken)
        {
            var entity = new Payment
            {
                EnrollmentId = request.EnrollmentId,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
                Method = request.Method,
                Status = PaymentStatus.Pending
            };

            _paymentRepository.Add(entity);

            return Task.FromResult(entity);
        }
    }
}
