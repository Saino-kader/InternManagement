using InterManagement.Application.Features.WeeklyFollowUps.Commands.CreateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.CompleteFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.MarkMissed;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.DeleteWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUps;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById;
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class WeeklyFollowUpController : ControllerBase
    {
        private readonly CreateWeeklyFollowUpHandler  _createHandler;
        private readonly CompleteFollowUpHandler      _completeHandler;
        private readonly MarkMissedHandler            _missedHandler;
        private readonly DeleteWeeklyFollowUpHandler  _deleteHandler;
        private readonly GetWeeklyFollowUpsHandler    _getHandler;
        private readonly GetWeeklyFollowUpByIdHandler _getByIdHandler;
        private readonly UpdateWeeklyFollowUpHandler  _updateHandler;

        public WeeklyFollowUpController(
            CreateWeeklyFollowUpHandler  createHandler,
            CompleteFollowUpHandler      completeHandler,
            MarkMissedHandler            missedHandler,
            DeleteWeeklyFollowUpHandler  deleteHandler,
            GetWeeklyFollowUpsHandler    getHandler,
            GetWeeklyFollowUpByIdHandler getByIdHandler,
            UpdateWeeklyFollowUpHandler  updateHandler)
        {
            _createHandler   = createHandler;
            _completeHandler = completeHandler;
            _missedHandler   = missedHandler;
            _deleteHandler   = deleteHandler;
            _getHandler      = getHandler;
            _getByIdHandler  = getByIdHandler;
            _updateHandler   = updateHandler;
        }

        // GET api/weeklyfollowup
        // GET api/weeklyfollowup?phaseId=5
        // GET api/weeklyfollowup?mentorId=3
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? phaseId,
            [FromQuery] int? mentorId)
        {
            var query = new GetWeeklyFollowUpsQuery
            {
                PhaseId  = phaseId,
                MentorId = mentorId
            };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/weeklyfollowup/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetWeeklyFollowUpByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/weeklyfollowup
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateWeeklyFollowUpDto dto)
        {
            var command = new CreateWeeklyFollowUpCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // PUT api/weeklyfollowup/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(
            int id,
            [FromBody] string comment)
        {
            var command = new CompleteFollowUpCommand(id, comment);
            await _completeHandler.Handle(command);
            return NoContent();
        }

        // PUT api/weeklyfollowup/5/missed
        [HttpPut("{id}/missed")]
        public async Task<IActionResult> MarkMissed(int id)
        {
            var command = new MarkMissedCommand(id);
            await _missedHandler.Handle(command);
            return NoContent();
        }

        // PUT api/weeklyfollowup/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateWeeklyFollowUpDto dto)
        {
            var command = new UpdateWeeklyFollowUpCommand(id, dto);
            await _updateHandler.Handle(command);
            return NoContent();
        }

        // DELETE api/weeklyfollowup/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteWeeklyFollowUpCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}
