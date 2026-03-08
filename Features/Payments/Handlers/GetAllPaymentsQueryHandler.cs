using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Features.Payments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.Features.Payments.Handlers
{
    public class GetAllPaymentsQueryHandler
        : IRequestHandler<GetAllPaymentsQuery, IEnumerable<PaymentViewModel>>
    {
        private readonly IGeneralRepository<Payment> _paymentRepository;

        public GetAllPaymentsQueryHandler(IGeneralRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentViewModel>> Handle(
            GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository
                .GetTable()
                .Include(p => p.Enrollment)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return payments.Select(p => p.ToDto().ToViewModel());
        }
    }
}
