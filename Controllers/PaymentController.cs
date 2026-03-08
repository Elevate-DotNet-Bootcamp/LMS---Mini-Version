using LMS___Mini_Version.Features.Payments.Queries;
using LMS___Mini_Version.ViewModels.Payment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [CQRS Fix] Read-only controller for Payment data.
    /// Injects ONLY IMediator — no more IPaymentService.
    /// Payments are created through the EnrollInternOrchestrator — not directly.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentViewModel>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllPaymentsQuery());
            return Ok(result);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<PaymentViewModel>> GetByEnrollment(int enrollmentId)
        {
            var result = await _mediator
                .Send(new GetPaymentByEnrollmentQuery(enrollmentId))
                .ConfigureAwait(false);

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentViewModel>> GetById(int id)
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 6: GetPaymentByIdQuery
            // ══════════════════════════════════════════════════════════════
            // TODO: The handler logic has not been implemented yet.
            // Inject IMediator in the constructor and return the result using:
            // await _mediator.Send(new GetPaymentByIdQuery(id));
            //
            // But first, implement the handler logic inside
            // GetPaymentByIdQueryHandler to find a payment by its Id.
            // ══════════════════════════════════════════════════════════════
            var result = await _mediator.Send(new GetPaymentByIdQuery(id)).ConfigureAwait(false);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<PaymentViewModel>>> GetPending()
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 7: GetPendingPaymentsQuery
            // ══════════════════════════════════════════════════════════════
            // TODO: The handler logic has not been implemented yet.
            // Inject IMediator in the constructor and return the result using:
            // await _mediator.Send(new GetPendingPaymentsQuery());
            //
            // But first, implement the handler logic inside
            // GetPendingPaymentsQueryHandler to filter payments by Pending status.
            // ══════════════════════════════════════════════════════════════
            var result = await _mediator.Send(new GetPendingPaymentsQuery()).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
