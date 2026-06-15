using InterManagement.Application.Features.InternFiles.Commands.CreateInternFile;
using InterManagement.Application.Features.InternFiles.Commands.DeleteInternFile;
using InterManagement.Application.Features.InternFiles.Queries.GetInternFiles;
using InterManagement.Application.Features.InternFiles.Queries.GetInternFileById;
using InterManagement.Application.Features.InternFiles.DTOs;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using InterManagement.Application.Features.InternFiles.Commands.UpdateInternFile;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternFileController : ControllerBase
    {
        private readonly CreateInternFileHandler  _createHandler;
        private readonly DeleteInternFileHandler  _deleteHandler;
        private readonly GetInternFilesHandler    _getHandler;
        private readonly GetInternFileByIdHandler _getByIdHandler;
        private readonly UpdateInternFileHandler _updateHandler;

        public InternFileController(
            CreateInternFileHandler  createHandler,
            DeleteInternFileHandler  deleteHandler,
            GetInternFilesHandler    getHandler,
            GetInternFileByIdHandler getByIdHandler,
            UpdateInternFileHandler updateHandler)
        {
            _createHandler  = createHandler;
            _deleteHandler  = deleteHandler;
            _getHandler     = getHandler;
            _getByIdHandler = getByIdHandler;
            _updateHandler  = updateHandler;
        }

        // GET api/internfile
        // GET api/internfile?traineeId=5
        // GET api/internfile?traineeId=5&fileType=PDF
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? traineeId,
            [FromQuery] string? fileType)
        {
            var query = new GetInternFilesQuery
            {
                TraineeId = traineeId,
                FileType  = fileType
            };
            var result = await _getHandler.Handle(query);
            return Ok(result);
        }

        // GET api/internfile/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query  = new GetInternFileByIdQuery(id);
            var result = await _getByIdHandler.Handle(query);
            return Ok(result);
        }

        // POST api/internfile
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateInternFileDto dto)
        {
            var command = new CreateInternFileCommand(dto);
            var result  = await _createHandler.Handle(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateInternFileDto dto)
        {
            var command = new UpdateInternFileCommand(id, dto);
            var result  = await _updateHandler.Handle(command);
            return Ok(result);
        }

        // DELETE api/internfile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteInternFileCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}
