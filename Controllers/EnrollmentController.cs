using LMS___Mini_Version.Features.Enrollments.Orchestrators;
using LMS___Mini_Version.Features.Enrollments.Queries;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [CQRS Fix] This controller injects ONLY IMediator.
    /// 
    /// Before (The Trap):  4 dependencies — IEnrollmentService + 3 manual Mediators
    /// After  (The Fix):   1 dependency  — IMediator
    ///
    /// Read operations dispatch Queries.
    /// Write operations dispatch Orchestrator Requests (which coordinate atomic steps internally).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ═══════════════════════════════════════════════════════
        //  READ ENDPOINTS (dispatched as Queries)
        // ═══════════════════════════════════════════════════════

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentViewModel>>> GetAll()
        {
            var result = await _mediator
                .Send(new GetAllEnrollmentsQuery())
                .ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentViewModel>> GetById(int id)
        {
            var result = await _mediator
                .Send(new GetEnrollmentByIdQuery(id))
                .ConfigureAwait(false);

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("intern/{internId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentViewModel>>> GetByIntern(int internId)
        {
            var result = await _mediator
                .Send(new GetEnrollmentsByInternQuery(internId))
                .ConfigureAwait(false);

            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════
        //  ACTION ENDPOINTS (dispatched as Orchestrator Requests)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Enrolls an intern in a track.
        /// The EnrollInternOrchestratorHandler coordinates all steps internally.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EnrollmentViewModel>> Enroll(EnrollInternViewModel vm)
        {
            var result = await _mediator
                .Send(new EnrollInternOrchestratorRequest(vm.InternId, vm.TrackId))
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result.Enrollment!.ToViewModel());
        }

        /// <summary>
        /// Cancels an enrollment and refunds the payment.
        /// The CancelEnrollmentOrchestratorHandler coordinates all steps internally.
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(int id)
        {
            var result = await _mediator
                .Send(new CancelEnrollmentOrchestratorRequest(id))
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Transfers an enrollment to a different track.
        /// The TransferEnrollmentOrchestratorHandler coordinates all steps internally.
        /// </summary>
        [HttpPost("{id}/transfer/{newTrackId}")]
        public async Task<ActionResult> Transfer(int id, int newTrackId)
        {
            var result = await _mediator
                .Send(new TransferEnrollmentOrchestratorRequest(id, newTrackId))
                .ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
