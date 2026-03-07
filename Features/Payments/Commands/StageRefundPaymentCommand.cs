using MediatR;

namespace LMS___Mini_Version.Features.Payments.Commands
{
    /// <summary>
    /// Atomic Step — marks a payment as Refunded in the Change Tracker.
    /// Does NOT call SaveChanges.
    /// </summary>
    public record StageRefundPaymentCommand(int EnrollmentId) : IRequest<bool>;
}
