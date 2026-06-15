using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class InternFile : BaseModel
    {
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public string FileType { get; private set; } = string.Empty;
        public DateTime ImportedAt { get; private set; }

        // ── FK 
        public int TraineeId { get; private set; }
        public Trainee Trainee { get; private set; } = null!;

        private InternFile() { }

        public InternFile(
            string fileName,
            string fileType,
            int traineeId)
        {
            // ── Validations 
            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("Le nom du fichier est obligatoire");


            if (string.IsNullOrWhiteSpace(fileType))
                throw new DomainException("Le type du fichier est obligatoire");

            if (traineeId <= 0)
                throw new DomainException("Le nom du stagiaire est obligatoire");




            // ── Validations type fichier 
            var allowedTypes = new[] { ".xls", ".xlsx", ".xlsm", ".csv" };
            if (!allowedTypes.Contains(fileType.ToUpper()))   // ToUpper : Convertit en MAJUSCULES  et Contains(...)	: Vérifie si la valeur est dans la liste autorisée
                throw new DomainException(
                    $"Le type de fichier {fileType} n'est pas autorisé. " +
                    $"Types autorisés: {string.Join(", ", allowedTypes)}"); // string.Join(", ", allowedTypes) Transforme la liste en texte avec des virgules

            // ── Assignation 
            FileName   = fileName;
            FilePath   = string.Empty;
            FileType   = fileType.ToUpper();
            TraineeId  = traineeId;
            ImportedAt = DateTime.UtcNow;
        }

        // ── Méthode métier 


        
        public void SetFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainException("File path is required");

            FilePath  = filePath;
            UpdatedAt = DateTime.UtcNow;
        }
        

        public void UpdateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("File name is required");

            FileName  = fileName;
            UpdatedAt = DateTime.UtcNow;
        }

        /*
        public void UpdateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new DomainException("File path is required");

            FilePath  = filePath;
            UpdatedAt = DateTime.UtcNow;
        }
        */
    }

}