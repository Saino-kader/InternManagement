namespace InterManagement.Application.Features.InternFiles.Commands.SetFilePath
{
    public class SetFilePathCommand
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;

        public SetFilePathCommand(int id, string filePath)
        {
            Id       = id;
            FilePath = filePath;
        }
    }
}