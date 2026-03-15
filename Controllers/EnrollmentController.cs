using LMS___Mini_Version.CQRS.Enrollments.Queries;
using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Mediators;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.ViewModels.Enrollment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [Trap 5 Fix] The POST action delegates to EnrollInternMediator — the action coordinator.
    ///              The Controller does NOT orchestrate multi-step business logic itself.
    /// [Trap 6 Fix] The Mediator handles the atomic commit via UoW.CompleteAsync().
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly EnrollInternMediator _mmediator;
        private readonly IMediator _mediator;
        public EnrollmentController(
            IEnrollmentService enrollmentService,
            EnrollInternMediator mmediator,
            IMediator mediator)
        {
            _enrollmentService = enrollmentService;
            _mmediator = mmediator;
            _mediator = mediator;
        }

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

        /// <summary>
        /// Enrolls an intern in a track. This is a multi-step action orchestrated by the Mediator:
        ///   1. Validates intern & track
        ///   2. Checks capacity
        ///   3. Creates enrollment + payment (if paid track)
        ///   4. Commits atomically via UoW
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EnrollmentViewModel>> Enroll(EnrollInternViewModel vm)
        {
            var result = await _mmediator.ExecuteAsync(new CreateEnrollmentDto
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
        [HttpGet("by-intern/{id})")]
        public async Task<ActionResult<IEnumerable<EnrollmentViewModel>>> GetEnrollmentByInternId(int id)
        {
            var enrollmentDtos = await _mediator.Send(new GetEnrollmentsByInternQuery(id));
            var viewModels = enrollmentDtos.Select(s => s.ToViewModel());

            return Ok(viewModels);
        }
    }
}
