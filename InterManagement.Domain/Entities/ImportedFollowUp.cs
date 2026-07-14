// Domain/Entities/ImportedFollowUp.cs
//
// Entité qui stocke les données brutes importées
// depuis un fichier Excel, sans lien avec les autres
// entités (Trainee, Mentor, Week...).
// Les données sont stockées en texte brut exactement
// comme elles apparaissent dans le fichier Excel.

using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class ImportedFollowUp : BaseModel
    {
        // Données brutes du fichier Excel (texte brut)
        public string Stagiaire   { get; private set; } = string.Empty;
        public string Mentor      { get; private set; } = string.Empty;
        public DateOnly Date      { get; private set; }
        public int WeekNumber     { get; private set; }
        public string Cours       { get; private set; } = string.Empty;
        public string Appreciation { get; private set; } = string.Empty;
        public string Commentaire { get; private set; } = string.Empty;
        public string Statut      { get; private set; } = string.Empty;

        // Métadonnées de l'import
        public DateTime ImportedAt { get; private set; }
        public string BatchId      { get; private set; } = string.Empty;

        private ImportedFollowUp() { }

        public ImportedFollowUp(
            string stagiaire,
            string mentor,
            DateOnly date,
            int weekNumber,
            string cours,
            string appreciation,
            string commentaire,
            string statut,
            string batchId)
        {
            if (string.IsNullOrWhiteSpace(stagiaire))
                throw new DomainException("Le nom du stagiaire est obligatoire");

            if (weekNumber <= 0)
                throw new DomainException("Le numéro de semaine doit être un entier positif");

            Stagiaire    = stagiaire.Trim();
            Mentor       = mentor?.Trim() ?? "—";
            Date         = date;
            WeekNumber   = weekNumber;
            Cours        = cours?.Trim() ?? "—";
            Appreciation = appreciation?.Trim() ?? "—";
            Commentaire  = commentaire?.Trim() ?? string.Empty;
            Statut       = statut?.Trim() ?? "—";
            ImportedAt   = DateTime.UtcNow;
            BatchId      = batchId;
        }

        public void Update(
            string cours,
            string appreciation,
            string commentaire,
            string statut)
        {
            Cours        = cours?.Trim() ?? Cours;
            Appreciation = appreciation?.Trim() ?? Appreciation;
            Commentaire  = commentaire?.Trim() ?? Commentaire;
            Statut       = statut?.Trim() ?? Statut;
            UpdatedAt    = DateTime.UtcNow;
        }
    }
}
