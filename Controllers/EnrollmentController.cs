using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Mediators;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.ViewModels.Enrollment;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// ╔═══════════════════════════════════════════════════════════════════════════════════╗
    /// ║  [THE FINAL TRAP: Mediator Explosion / Constructor Over-Injection]                ║
    /// ╠═══════════════════════════════════════════════════════════════════════════════════╣
    /// ║                                                                                   ║
    /// ║  Look at the constructor below!                                                   ║
    /// ║                                                                                   ║
    /// ║  Because we created a "Class per Action" Mediator pattern,                        ║
    /// ║  this controller now has to inject a DIFFERENT Mediator for                       ║
    /// ║  EVERY complex business action:                                                   ║
    /// ║                                                                                   ║
    /// ║    - EnrollInternMediator      → POST  /api/enrollment                            ║
    /// ║    - CancelEnrollmentMediator  → POST  /api/enrollment/{id}/cancel                ║
    /// ║    - TransferEnrollmentMediator→ POST  /api/enrollment/{id}/transfer/{newTrackId} ║
    /// ║                                                                                   ║
    /// ║  Every NEW business action = another constructor parameter.                       ║
    /// ║  This violates the Open/Closed Principle: adding a new action                     ║
    /// ║  FORCES us to modify this controller's constructor.                               ║
    /// ║                                                                                   ║
    /// ║  As the system grows, the constructor will bloat with 10+                         ║
    /// ║  mediators, making the class hard to maintain and test.                           ║
    /// ║                                                                                   ║
    /// ║  ► The REAL Solution: Replace all these manual mediators with                     ║
    /// ║    the CQRS pattern using MediatR library, where each action                      ║
    /// ║    becomes a self-contained Command/Query that is dispatched                      ║
    /// ║    through a SINGLE IMediator interface:                                          ║
    /// ║                                                                                   ║
    /// ║    Before: 4 constructor parameters (and growing)                                 ║
    /// ║    After:  1 constructor parameter → IMediator                                    ║
    /// ║                                                                                   ║
    /// ╚═══════════════════════════════════════════════════════════════════════════════════╝
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        // ⚠️ THE TRAP: 4 dependencies and counting — every new action adds another one!
        private readonly IEnrollmentService _enrollmentService;
        private readonly EnrollInternMediator _enrollMediator;
        private readonly CancelEnrollmentMediator _cancelMediator;
        private readonly TransferEnrollmentMediator _transferMediator;

        // ⚠️ Constructor bloat — imagine this with 10+ business actions!
        public EnrollmentController(
            IEnrollmentService enrollmentService,
            EnrollInternMediator enrollMediator,
            CancelEnrollmentMediator cancelMediator,
            TransferEnrollmentMediator transferMediator)
        {
            _enrollmentService = enrollmentService;
            _enrollMediator = enrollMediator;
            _cancelMediator = cancelMediator;
            _transferMediator = transferMediator;
        }

        // ═══════════════════════════════════════════════════════
        //  READ ENDPOINTS (delegated to IEnrollmentService)
        // ═══════════════════════════════════════════════════════

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentViewModel>>> GetAll()
        {
            var dtos = await _enrollmentService.GetAllAsync().ConfigureAwait(false);
            var viewModels = dtos.Select(d => d.ToViewModel());
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentViewModel>> GetById(int id)
        {
            var dto = await _enrollmentService.GetByIdAsync(id).ConfigureAwait(false);
            if (dto == null) return NotFound();
            return Ok(dto.ToViewModel());
        }

        [HttpGet("intern/{internId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentViewModel>>> GetByIntern(int internId)
        {
            var dtos = await _enrollmentService.GetByInternAsync(internId).ConfigureAwait(false);
            var viewModels = dtos.Select(d => d.ToViewModel());
            return Ok(viewModels);
        }

        // ═══════════════════════════════════════════════════════
        //  ACTION ENDPOINTS (each delegated to its own Mediator)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Enrolls an intern in a track.
        /// Orchestrated by EnrollInternMediator (validates → creates enrollment → creates payment → commits).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EnrollmentViewModel>> Enroll(EnrollInternViewModel vm)
        {
            var result = await _enrollMediator.ExecuteAsync(new CreateEnrollmentDto
            {
                InternId = vm.InternId,
                TrackId = vm.TrackId
            }).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result.Enrollment!.ToViewModel());
        }

        /// <summary>
        /// Cancels an enrollment and refunds the payment.
        /// Orchestrated by CancelEnrollmentMediator (cancels → refunds → commits).
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(int id)
        {
            var result = await _cancelMediator.ExecuteAsync(id).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Transfers an enrollment to a different track and adjusts the payment.
        /// Orchestrated by TransferEnrollmentMediator (validates → transfers → adjusts fees → commits).
        /// </summary>
        [HttpPost("{id}/transfer/{newTrackId}")]
        public async Task<ActionResult> Transfer(int id, int newTrackId)
        {
            var result = await _transferMediator.ExecuteAsync(id, newTrackId).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
