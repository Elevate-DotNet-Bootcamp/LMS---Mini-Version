using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Payments.Queries.QueryHandler
{
    public class GetPendingPaymentsQueryHandler : IRequestHandler<GetPendingPaymentsQuery, IEnumerable<PaymentDto>>
    {
        IGeneralRepository<Payment> _paymentRepo;
        public GetPendingPaymentsQueryHandler(IGeneralRepository<Payment> paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }
        public async Task<IEnumerable<PaymentDto>> Handle(GetPendingPaymentsQuery request, CancellationToken cancellationToken)
        {
            return await _paymentRepo.GetTable().Where(p => p.Status == PaymentStatus.Pending)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    EnrollmentId = p.EnrollmentId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Method = p.Method,
                    Status = p.Status
                }).ToListAsync(cancellationToken);
        }
    }
}
