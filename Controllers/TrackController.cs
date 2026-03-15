using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.ViewModels.Track;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using LMS___Mini_Version.CQRS.Tracks.Queries;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [Trap 1 Fix] This controller NO LONGER injects AppDbContext.
    ///              It depends only on ITrackService and IUnitOfWork (abstractions).
    /// [Trap 2 Fix] All responses use ViewModels; all inputs use ViewModels.
    ///              The domain entity "Track" is never exposed to the client.
    /// [Trap 3 Fix] Every action is async Task — no synchronous blocking.
    /// [Trap 5 Fix] No business logic in the controller — all delegated to TrackService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public TrackController(ITrackService trackService, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _trackService = trackService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackSummaryViewModel>>> GetAll()
        {
            var dtos = await _trackService.GetAllAsync().ConfigureAwait(false);
            var viewModels = dtos.Select(d => d.ToSummaryViewModel());
            return Ok(viewModels);
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<TrackDetailViewModel>> GetById(int id)
        //{
        //    var dto = await _trackService.GetByIdAsync(id).ConfigureAwait(false);
        //    if (dto == null) return NotFound();
        //    return Ok(dto.ToDetailViewModel());
        //}

        [HttpGet("ActiveTracks")]
        public async Task<ActionResult<IEnumerable<TrackDetailViewModel>>> GetActvieTracks()
        {
            var dtos = await _mediator.Send(new GetActiveTracksQuery());

            return Ok(dtos.Select(d => d.ToDetailViewModel()));
        }

        [HttpPost]
        public async Task<ActionResult<TrackSummaryViewModel>> Create(CreateTrackViewModel vm)
        {
            var dto = new TrackDto
            {
                Name = vm.Name,
                Fees = vm.Fees,
                IsActive = vm.IsActive,
                MaxCapacity = vm.MaxCapacity
            };

            var created = await _trackService.CreateAsync(dto).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(created.ToSummaryViewModel());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateTrackViewModel vm)
        {
            var dto = new TrackDto
            {
                Name = vm.Name,
                Fees = vm.Fees,
                IsActive = vm.IsActive,
                MaxCapacity = vm.MaxCapacity
            };

            var updated = await _trackService.UpdateAsync(id, dto).ConfigureAwait(false);
            if (!updated) return NotFound();

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _trackService.DeleteAsync(id).ConfigureAwait(false);
            if (!deleted) return NotFound();

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return NoContent();
        }

        [HttpGet("NewGetTrack")]
        public async Task<ActionResult> GetTrackData([FromQuery] int id)
        {
            var trackData = await _mediator.Send(new GetTrackByIdQuery(id));
            var trackViewModel = trackData?.ToDetailViewModel();

            return Ok(trackViewModel);
        }
    }
}