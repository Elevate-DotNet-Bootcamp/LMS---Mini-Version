using LMS___Mini_Version.DTOs;
using MediatR;

namespace LMS___Mini_Version.CQRS.Payments.Queries
{
    public record GetPendingPaymentsQuery:IRequest<IEnumerable<PaymentDto>>;
}
    