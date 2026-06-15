using InterManagement.Application.Features.Weeks.Commands.CreateWeek;
using InterManagement.Application.Features.Weeks.Commands.UpdateWeek;
using InterManagement.Application.Features.Weeks.Commands.DeleteWeek;
using InterManagement.Application.Features.Weeks.Queries.GetWeeks;
using InterManagement.Application.Features.Weeks.Queries.GetWeekById;
using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeekController : ControllerBase
    {
        private readonly CreateWeekHandler  _createHandler;
        private readonly UpdateWeekHandler  _updateHandler;
        private readonly DeleteWeekHandler  _deleteHandler;
        private readonly GetWeeksHandler    _getHandler;
        private readonly GetWeekByIdHandler _getByIdHandler;

        public WeekController(
            CreateWeekHandler  createHandler,
            UpdateWeekHandler  updateHandler,
            DeleteWeekHandler  deleteHandler,
            GetWeeksHandler    getHandler,
            GetWeekByIdHandler getByIdHandler)
        {
            _createHandler  = createHandler;
            _updateHandler  = updateHandler;
            _deleteHandler  = deleteHandler;
            _getHandler     = getHandler;
            _getByIdHandler = getByIdHandler;
        }

        // GET api/week
        // GET api/week?phaseId=5
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? phaseId)
        {
            var query  = new GetWeeksQuery { PhaseId = phaseId };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/week/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetWeekByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/week
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateWeekDto dto)
        {
            var command = new CreateWeekCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // PUT api/week/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateWeekDto dto)
        {
            var command = new UpdateWeekCommand(id, dto);
            var result  = await _updateHandler.Handle(command);
            return Ok(result);
        }

        // DELETE api/week/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteWeekCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}