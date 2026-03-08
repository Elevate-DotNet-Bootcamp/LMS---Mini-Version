using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Payments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    public class GetPaymentByEnrollmentQueryHandler
        : IRequestHandler<GetPaymentByEnrollmentQuery, PaymentViewModel?>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public GetPaymentByEnrollmentQueryHandler(
            IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentViewModel?> Handle(
            GetPaymentByEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository
                .GetTable()
                .FirstOrDefaultAsync(p => p.EnrollmentId == request.EnrollmentId,
                    cancellationToken)
                .ConfigureAwait(false);

            return payment?.ToDto().ToViewModel();
        }
    }
}
