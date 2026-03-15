using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.CQRS.Payments.Queries.QueryHandler
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
    {
        IGeneralRepository<Payment> _paymentRepo;
        public GetPaymentByIdQueryHandler(IGeneralRepository<Payment> paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }
        public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _paymentRepo.GetTable().Where(p => p.Id == request.id).Select(p => new PaymentDto
            {
                Id = p.Id,
                EnrollmentId = p.EnrollmentId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Method = p.Method,
                Status = p.Status
            }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
