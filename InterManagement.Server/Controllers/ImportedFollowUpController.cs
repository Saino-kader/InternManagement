// Server/Controllers/ImportedFollowUpController.cs
using InterManagement.Application.Features.ImportedFollowUps.Commands.DeleteImportedFollowUp;
using InterManagement.Application.Features.ImportedFollowUps.Commands.ImportExcel;
using InterManagement.Application.Features.ImportedFollowUps.Commands.UpdateImportedFollowUp;
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Application.Features.ImportedFollowUps.Queries.GetImportedFollowUps;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportedFollowUpController : ControllerBase
    {
        private readonly GetImportedFollowUpsHandler    _getHandler;
        private readonly ImportExcelHandler             _importHandler;
        private readonly UpdateImportedFollowUpHandler  _updateHandler;
        private readonly DeleteImportedFollowUpHandler  _deleteHandler;

        public ImportedFollowUpController(
            GetImportedFollowUpsHandler    getHandler,
            ImportExcelHandler             importHandler,
            UpdateImportedFollowUpHandler  updateHandler,
            DeleteImportedFollowUpHandler  deleteHandler)
        {
            _getHandler    = getHandler;
            _importHandler = importHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        // GET /api/importedfollowup
        // Retourne tous les suivis importés
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getHandler.Handle(new GetImportedFollowUpsQuery());
            return Ok(result);
        }

        // POST /api/importedfollowup/import
        // Importe un fichier Excel → stocke dans imported_followups
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Fichier obligatoire" });

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!new[] { ".xlsx", ".xls", ".xlsm" }.Contains(extension))
                return BadRequest(new { error = "Format non supporté. Utilisez .xlsx" });

            using var stream = file.OpenReadStream();
            var command = new ImportExcelCommand
            {
                FileStream    = stream,
                FileExtension = extension
            };

            var result = await _importHandler.Handle(command);
            return Ok(result);
        }

        // PUT /api/importedfollowup/{id}
        // Modifie un suivi importé
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateImportedFollowUpDto dto)
        {
            var command = new UpdateImportedFollowUpCommand { Id = id, Data = dto };
            var result  = await _updateHandler.Handle(command);
            return Ok(result);
        }

        // DELETE /api/importedfollowup/{id}
        // Supprime un suivi importé
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteImportedFollowUpCommand(id);
            await _deleteHandler.Handle(command);
            return NoContent();
        }
    }
}
