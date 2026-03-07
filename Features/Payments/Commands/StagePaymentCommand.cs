using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using MediatR;

namespace LMS___Mini_Version.Features.Payments.Commands
{
    /// <summary>
    /// Atomic Step — stages a new Payment in the EF Change Tracker.
    /// Does NOT call SaveChanges. The Orchestrator commits via IUnitOfWork.
    /// Returns the tracked Payment entity.
    /// </summary>
    public record StagePaymentCommand(
        int EnrollmentId,
        decimal Amount,
        PaymentMethod Method
    ) : IRequest<Payment>;
}
