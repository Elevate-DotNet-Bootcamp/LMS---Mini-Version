using LMS___Mini_Version.Features.Interns.Commands;
using LMS___Mini_Version.Features.Interns.Queries;
using LMS___Mini_Version.ViewModels.Intern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [CQRS Fix] Injects ONLY IMediator — no more IInternService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InternController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternSummaryViewModel>>> GetAll()
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 2: GetAllInternsQuery
            // ══════════════════════════════════════════════════════════════
            // The handler logic has been removed. You need to:
            // 1) Implement the business logic inside GetAllInternsQueryHandler
            // 2) The controller is already wired — just fix the handler!
            // ══════════════════════════════════════════════════════════════
            var result = await _mediator.Send(new GetAllInternsQuery()).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InternDetailViewModel>> GetById(int id)
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 3: GetInternByIdQuery
            // ══════════════════════════════════════════════════════════════
            // The handler logic has been removed. You need to:
            // 1) Implement the business logic inside GetInternByIdQueryHandler
            // 2) The controller is already wired — just fix the handler!
            // ══════════════════════════════════════════════════════════════
            throw new NotImplementedException();
        }
        #region
        //var result = await _mediator.Send(new GetInternByIdQuery(id)).ConfigureAwait(false);
        //if (result == null) return NotFound();
        //return Ok(result);
        #endregion
        [HttpPost]
        public async Task<ActionResult<InternSummaryViewModel>> Create(CreateInternViewModel vm)
        {
            var result = await _mediator.Send(new CreateInternCommand(
                vm.FullName, vm.Email, vm.BirthYear, vm.Status, vm.TrackId
            )).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateInternViewModel vm)
        {
            var updated = await _mediator.Send(new UpdateInternCommand(
                id, vm.FullName, vm.Email, vm.BirthYear, vm.Status, vm.TrackId
            )).ConfigureAwait(false);

            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeleteInternCommand(id)).ConfigureAwait(false);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}