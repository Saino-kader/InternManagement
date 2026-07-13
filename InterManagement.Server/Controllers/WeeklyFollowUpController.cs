using InterManagement.Application.Features.WeeklyFollowUps.Commands.CreateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ValidatedFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.SuspendedCommand;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.DeleteWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUps;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById;
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.ExportWeeklyFollowUpsByTrainee;


namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class WeeklyFollowUpController : ControllerBase
    {
        private readonly CreateWeeklyFollowUpHandler  _createHandler;
        private readonly ValidatedFollowUpHandler      _completeHandler;
        private readonly SuspendedHandler            _suspendedHandler;
        private readonly DeleteWeeklyFollowUpHandler  _deleteHandler;
        private readonly GetWeeklyFollowUpsHandler    _getHandler;
        private readonly GetWeeklyFollowUpByIdHandler _getByIdHandler;
        private readonly UpdateWeeklyFollowUpHandler  _updateHandler;
        private readonly ImportWeeklyFollowUpsHandler _importHandler;
        private readonly ExportWeeklyFollowUpsByTraineeHandler _exportHandler;

        public WeeklyFollowUpController(
            CreateWeeklyFollowUpHandler  createHandler,
            ValidatedFollowUpHandler      ValidatedHandler,
            SuspendedHandler            suspendedHandler,
            DeleteWeeklyFollowUpHandler  deleteHandler,
            GetWeeklyFollowUpsHandler    getHandler,
            GetWeeklyFollowUpByIdHandler getByIdHandler,
            UpdateWeeklyFollowUpHandler  updateHandler,
            ImportWeeklyFollowUpsHandler importHandler,
            ExportWeeklyFollowUpsByTraineeHandler exportHandler)
        {
            _createHandler   = createHandler;
            _completeHandler = ValidatedHandler;
            _suspendedHandler   = suspendedHandler;
            _deleteHandler   = deleteHandler;
            _getHandler      = getHandler;
            _getByIdHandler  = getByIdHandler;
            _updateHandler   = updateHandler;
            _importHandler   = importHandler;
            _exportHandler   = exportHandler;
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
        public async Task<IActionResult> Validated(
            int id,
            [FromBody] string comment)
        {
            var command = new ValidatedFollowUpCommand(id);
            await _completeHandler.Handle(command);
            return NoContent();
        }

        // PUT api/weeklyfollowup/5/missed
        [HttpPut("{id}/missed")]
        public async Task<IActionResult> MarkMissed(int id)
        {
            var command = new SuspendedCommand(id);
            await _suspendedHandler.Handle(command);
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

        // POST api/weeklyfollowup/import
        // Accepte 1 à plusieurs stagiaires dans le même fichier
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var extension = Path.GetExtension(file.FileName);
            using var stream = file.OpenReadStream();
            var command = new ImportWeeklyFollowUpsCommand(stream, extension);
            var result = await _importHandler.Handle(command);
            return Ok(result);
        }

        // GET api/weeklyfollowup/export/{traineeId}
        // Exporte UN SEUL stagiaire
        [HttpGet("export/{traineeId}")]
        public async Task<IActionResult> Export(int traineeId)
        {
            var query = new ExportWeeklyFollowUpsByTraineeQuery(traineeId);
            var result = await _exportHandler.Handle(query);
            return File(
                result.FileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.FileName);
        }
    }
}
