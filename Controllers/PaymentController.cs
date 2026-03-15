using LMS___Mini_Version.CQRS.Payments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.ViewModels.Payment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// Read-only controller for Payment data.
    /// Payments are created through the EnrollInternMediator — not directly.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMediator _mediator;
        public PaymentController(IPaymentService paymentService, IMediator mediator)
        {
            _paymentService = paymentService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentViewModel>>> GetAll()
        {
            var dtos = await _paymentService.GetAllAsync().ConfigureAwait(false);
            var viewModels = dtos.Select(d => d.ToViewModel());
            return Ok(viewModels);
        }

        [HttpGet("enrollment/{enrollmentId}")]
        public async Task<ActionResult<PaymentViewModel>> GetByEnrollment(int enrollmentId)
        {
            var dto = await _paymentService.GetByEnrollmentAsync(enrollmentId).ConfigureAwait(false);
            if (dto == null) return NotFound();
            return Ok(dto.ToViewModel());
        }
        [HttpGet("GetPayment/{PaymentId}")]
        public async Task<ActionResult<PaymentViewModel>> GetPaymentById(int PaymentId)
        {
            var dto = await _mediator.Send(new GetPaymentByIdQuery(PaymentId));
            if (dto == null) return NotFound();
            return Ok(dto.ToViewModel());
        }
        [HttpGet("GetPendingPayment")]
        public async Task<ActionResult<PaymentViewModel>> GetPendingPayment()
        {
            var dto = await _mediator.Send(new GetPendingPaymentsQuery());
            return Ok(dto.Select(d => d.ToViewModel()));
        }
    }
}
