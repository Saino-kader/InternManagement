using InterManagement.Application.Features.InternFiles.DTOs;

namespace InterManagement.Application.Features.InternFiles.Commands.UpdateInternFile
{
    public class UpdateInternFileCommand
    {
        public int Id { get; set; }
        public UpdateInternFileDto Data { get; set; }

        public UpdateInternFileCommand(int id, UpdateInternFileDto data)
        {
            Id   = id;
            Data = data;
        }
    }
}