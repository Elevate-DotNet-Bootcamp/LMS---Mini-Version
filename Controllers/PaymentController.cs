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
            var result = await _mediator.Send(new GetAllPaymentsQuery()).ConfigureAwait(false);
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
    }
}
