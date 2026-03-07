using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Payments.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    public class StageUpdatePaymentAmountCommandHandler
        : IRequestHandler<StageUpdatePaymentAmountCommand, bool>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public StageUpdatePaymentAmountCommandHandler(
            IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<bool> Handle(
            StageUpdatePaymentAmountCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository
                .GetTable()
                .FirstOrDefaultAsync(p => p.EnrollmentId == request.EnrollmentId, cancellationToken)
                .ConfigureAwait(false);

            if (payment == null) return false;

            payment.Amount = request.NewAmount;
            payment.Status = PaymentStatus.Pending;
            _paymentRepository.Update(payment);
            return true;
        }
    }
}
