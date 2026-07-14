using InterManagement.Application.Features.Assignments.Commands.CreateAssignment;
using InterManagement.Application.Features.Assignments.Commands.DeactivateAssignment;
using InterManagement.Application.Features.Assignments.Commands.DeleteAssignment;
using InterManagement.Application.Features.Assignments.Queries.GetAssignments;
using InterManagement.Application.Features.Assignments.Queries.GetAssignmentById;
using InterManagement.Application.Features.Assignments.Queries.GetMentorAssignments;
using InterManagement.Application.Features.Assignments.DTOs;
using InterManagement.Application.Features.Assignments.Queries.GetAssignmentsByMentor;

using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly CreateAssignmentHandler     _createHandler;
        private readonly DeactivateAssignmentHandler _deactivateHandler;
        private readonly DeleteAssignmentHandler     _deleteHandler;
        private readonly GetAssignmentsHandler       _getHandler;
        private readonly GetAssignmentByIdHandler    _getByIdHandler;
        private readonly GetMentorAssignmentsHandler _getMentorAssignmentsHandler;


        public AssignmentController(
            CreateAssignmentHandler     createHandler,
            DeactivateAssignmentHandler deactivateHandler,
            DeleteAssignmentHandler     deleteHandler,
            GetAssignmentsHandler       getHandler,
            GetAssignmentByIdHandler    getByIdHandler,
            GetMentorAssignmentsHandler getMentorAssignmentsHandler)
        {
            _createHandler     = createHandler;
            _deactivateHandler = deactivateHandler;
            _deleteHandler     = deleteHandler;
            _getHandler        = getHandler;
            _getByIdHandler    = getByIdHandler;
            _getMentorAssignmentsHandler = getMentorAssignmentsHandler;

        }

        // GET api/assignment
        // GET api/assignment?mentorId=5
        // GET api/assignment?traineeId=3
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? mentorId,
            [FromQuery] int? traineeId)
        {
            var query = new GetAssignmentsQuery
            {
                MentorId  = mentorId,
                TraineeId = traineeId
            };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/assignment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetAssignmentByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/assignment
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAssignmentDto dto)
        {
            var command = new CreateAssignmentCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // PUT api/assignment/5/deactivate
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var command = new DeactivateAssignmentCommand(id);
            await _deactivateHandler.Handle(command);
            return NoContent();
        }

        // DELETE api/assignment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteAssignmentCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }

        // GET api/assignment/mentor/{mentorId}/details
        // Retourne les assignments détaillés d'un mentor avec Trainee + Phase + Weeks
        [HttpGet("mentor/{mentorId}/details")]
        public async Task<IActionResult> GetMentorAssignmentsDetails(int mentorId)
        {
            var query = new GetMentorAssignmentsQuery(mentorId);
            var result = await _getMentorAssignmentsHandler.Handle(query);
            return Ok(result);
        }
    }
}
