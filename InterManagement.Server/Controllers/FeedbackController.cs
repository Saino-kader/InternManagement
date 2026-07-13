// Server/Controllers/FeedbackController.cs
using InterManagement.Application.Features.Feedbacks.Commands.CreateFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.UpdateFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.DeleteFeedback;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbacks;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbackById;
using InterManagement.Application.Features.Feedbacks.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly CreateFeedbackHandler  _createHandler;
        private readonly UpdateFeedbackHandler  _updateHandler;
        private readonly DeleteFeedbackHandler  _deleteHandler;
        private readonly GetFeedbacksHandler    _getHandler;
        private readonly GetFeedbackByIdHandler _getByIdHandler;

        public FeedbackController(
            CreateFeedbackHandler  createHandler,
            UpdateFeedbackHandler  updateHandler,
            DeleteFeedbackHandler  deleteHandler,
            GetFeedbacksHandler    getHandler,
            GetFeedbackByIdHandler getByIdHandler)
        {
            _createHandler  = createHandler;
            _updateHandler  = updateHandler;
            _deleteHandler  = deleteHandler;
            _getHandler     = getHandler;
            _getByIdHandler = getByIdHandler;
        }

        // GET api/feedback
        // GET api/feedback?traineeId=5
        // GET api/feedback?traineeId=5&count=3
        // GET api/feedback?mentorId=7      ← NOUVEAU
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? traineeId,
            [FromQuery] int? mentorId,    // ← AJOUTÉ
            [FromQuery] int? count)
        {
            var query = new GetFeedbacksQuery
            {
                TraineeId = traineeId,
                MentorId  = mentorId,     // ← AJOUTÉ
                Count     = count
            };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/feedback/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetFeedbackByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/feedback
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto dto)
        {
            var command = new CreateFeedbackCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/feedback/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateFeedbackDto dto)
        {
            var command = new UpdateFeedbackCommand(id, dto);
            var result  = await _updateHandler.Handle(command);
            return Ok(result);
        }

        // DELETE api/feedback/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteFeedbackCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}