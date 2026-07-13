using InterManagement.Application.Features.Phases.Commands.CreatePhase;
using InterManagement.Application.Features.Phases.Commands.UpdatePhase;
using InterManagement.Application.Features.Phases.Commands.DeletePhase;
using InterManagement.Application.Features.Phases.Queries.GetPhases;
using InterManagement.Application.Features.Phases.Queries.GetPhaseById;
using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhaseController : ControllerBase
    {
        private readonly CreatePhaseHandler  _createHandler;
        private readonly UpdatePhaseHandler  _updateHandler;
        private readonly DeletePhaseHandler  _deleteHandler;
        private readonly GetPhasesHandler    _getHandler;
        private readonly GetPhaseByIdHandler _getByIdHandler;
        private readonly CreatePhaseForMultipleTraineesHandler _multiCreateHandler;

        public PhaseController(
            CreatePhaseHandler  createHandler,
            UpdatePhaseHandler  updateHandler,
            DeletePhaseHandler  deleteHandler,
            GetPhasesHandler    getHandler,
            GetPhaseByIdHandler getByIdHandler,
            CreatePhaseForMultipleTraineesHandler multiCreateHandler)
        {
            _createHandler  = createHandler;
            _updateHandler  = updateHandler;
            _deleteHandler  = deleteHandler;
            _getHandler     = getHandler;
            _getByIdHandler = getByIdHandler;
            _multiCreateHandler = multiCreateHandler;
        }

        // GET api/phase
        // GET api/phase?traineeId=5
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? traineeId)
        {
            var query  = new GetPhasesQuery { TraineeId = traineeId };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/phase/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetPhaseByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/phase
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePhaseDto dto)
        {
            var command = new CreatePhaseCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // POST api/phase/multi-create
        [HttpPost("multi-create")]
        public async Task<IActionResult> CreateForMultipleTrainees(
            [FromBody] CreatePhaseForMultipleTraineesDto dto)
        {
            var command = new CreatePhaseForMultipleTraineesCommand(dto);
            var result = await _multiCreateHandler.Handle(command);
            return Ok(result);
        }


        // PUT api/phase/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePhaseDto dto)
        {
            var command = new UpdatePhaseCommand(id, dto);
            var result  = await _updateHandler.Handle(command);
            return Ok(result);
        }

        // DELETE api/phase/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePhaseCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}
