using LMS___Mini_Version.Features.Tracks.Commands;
using LMS___Mini_Version.Features.Tracks.Queries;
using LMS___Mini_Version.ViewModels.Track;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [CQRS Fix] This controller injects ONLY IMediator.
    /// All operations are dispatched as Commands (writes) or Queries (reads).
    /// No Services, no Repositories, no UnitOfWork — just a single mediator.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackSummaryViewModel>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllTracksQuery()).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDetailViewModel>> GetById(int id)
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 1: GetTrackByIdQuery
            // ══════════════════════════════════════════════════════════════
            // The handler logic has been removed. You need to:
            // 1) Implement the business logic inside GetTrackByIdQueryHandler
            // 2) The controller is already wired — just fix the handler!
            // ══════════════════════════════════════════════════════════════
            var trackdto = await _mediator.Send(new GetTrackByIdQuery(id));
            return trackdto == null ? NotFound() : Ok(trackdto);
        }
        #region answer
        //var result = await _mediator.Send(new GetTrackByIdQuery(id)).ConfigureAwait(false);
        //if (result == null) return NotFound();
        //return Ok(result);
        #endregion
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TrackSummaryViewModel>>> GetActiveTracks()
        {
            // ══════════════════════════════════════════════════════════════
            // 🎯 CQRS ASSIGNMENT — Task 4: GetActiveTracksQuery
            // ══════════════════════════════════════════════════════════════
            // TODO: The handler logic has not been implemented yet.
            // Inject IMediator in the constructor and return the result using:
            // await _mediator.Send(new GetActiveTracksQuery());
            //
            // But first, implement the handler logic inside
            // GetActiveTracksQueryHandler to query only active tracks.
            // ══════════════════════════════════════════════════════════════
            var activeTracks = await _mediator.Send(new GetActiveTracksQuery());
            return Ok(activeTracks);
        }
        #region
        //var result = await _mediator.Send(new GetActiveTracksQuery()).ConfigureAwait(false);
        //return Ok(result);
        #endregion
        [HttpPost]
        public async Task<ActionResult<TrackSummaryViewModel>> Create(CreateTrackViewModel vm)
        {
            var result = await _mediator.Send(new CreateTrackCommand(
                vm.Name, vm.Fees, vm.IsActive, vm.MaxCapacity
            )).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTrackViewModel vm)
        {
            var updated = await _mediator.Send(new UpdateTrackCommand(
                id, vm.Name, vm.Fees, vm.IsActive, vm.MaxCapacity
            )).ConfigureAwait(false);

            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeleteTrackCommand(id)).ConfigureAwait(false);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}