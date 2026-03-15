using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Payments.Queries
{
    public record GetPaymentByIdQuery(int id) : IRequest<PaymentDto>;
}
