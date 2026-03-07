using MediatR;

namespace LMS___Mini_Version.Features.Payments.Commands
{
    /// <summary>
    /// Atomic Step — updates payment amount in the Change Tracker.
    /// Used during track transfers when fees change.
    /// Does NOT call SaveChanges.
    /// </summary>
    public record StageUpdatePaymentAmountCommand(
        int EnrollmentId,
        decimal NewAmount
    ) : IRequest<bool>;
}
