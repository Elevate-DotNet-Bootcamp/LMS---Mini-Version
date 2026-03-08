using LMS___Mini_Version.ViewModels.Payment;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Queries
{
    public record GetAllPaymentsQuery : IRequest<IEnumerable<PaymentViewModel>>;
}
