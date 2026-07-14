namespace InterManagement.Domain.Entities
{
    /// <summary>
    /// Champs communs à toutes les entités persistées : identifiant,
    /// horodatage et suppression logique (soft delete).
    /// </summary>
    public class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}