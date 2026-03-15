using LMS___Mini_Version.DTOs;
using LMS___Mini_Version.Mapping;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.Services.Interfaces;
using LMS___Mini_Version.ViewModels.Intern;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using LMS___Mini_Version.CQRS.Tracks.Queries;
using LMS___Mini_Version.CQRS.Interns.Queries;

namespace LMS___Mini_Version.Controllers
{
    /// <summary>
    /// [Trap 1 Fix] Depends on IInternService + IUnitOfWork — NOT AppDbContext.
    /// [Trap 2 Fix] Accepts/returns ViewModels only.
    /// [Trap 3 Fix] Fully async.
    /// [Trap 5 Fix] Zero business logic — delegated to InternService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InternController : ControllerBase
    {
        private readonly IInternService _internService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public InternController(IInternService internService, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _internService = internService;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternSummaryViewModel>>> GetAll()
        {
            var dtos = await _internService.GetAllAsync().ConfigureAwait(false);
            var viewModels = dtos.Select(d => d.ToSummaryViewModel());
            return Ok(viewModels);
        }

        [HttpGet("AllIntern")]
        public async Task<ActionResult<IEnumerable<InternSummaryViewModel>>> GetAllIntern()
        {
            var dtos = await _mediator.Send(new GetAllInternQuery());
            var viewModels = dtos.Select(d => d.ToSummaryViewModel());
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InternDetailViewModel>> GetById(int id)
        {
            var dto = await _internService.GetByIdAsync(id).ConfigureAwait(false);
            if (dto == null) return NotFound();
            return Ok(dto.ToDetailViewModel());
        }

        [HttpPost]
        public async Task<ActionResult<InternSummaryViewModel>> Create(CreateInternViewModel vm)
        {
            var dto = new InternDto
            {
                FullName = vm.FullName,
                Email = vm.Email,
                BirthYear = vm.BirthYear,
                Status = vm.Status,
                TrackId = vm.TrackId
            };

            var created = await _internService.CreateAsync(dto).ConfigureAwait(false);
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);

            return Ok(created.ToSummaryViewModel());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateInternViewModel vm)
        {
            var dto = new InternDto
            {
                FullName = vm.FullName,
                Email = vm.Email,
                BirthYear = vm.BirthYear,
                Status = vm.Status,
                TrackId = vm.TrackId
            };

            var updated = await _internService.UpdateAsync(id, dto).ConfigureAwait(false);
            if (!updated) return NotFound();

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _internService.DeleteAsync(id).ConfigureAwait(false);
            if (!deleted) return NotFound();

            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
            return NoContent();
        }
        [HttpGet("InternDetails")]
        public async Task<ActionResult> GetInternById([FromQuery]int id)
        {
            var internDto = await _mediator.Send(new GetInternByIdQuery(id)); 
            if (internDto == null) return NotFound();

           var viewModel = internDto.ToDetailViewModel();
            return Ok(viewModel);
        }
    }
}