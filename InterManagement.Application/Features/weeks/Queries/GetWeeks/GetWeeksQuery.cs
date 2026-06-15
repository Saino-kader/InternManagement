namespace InterManagement.Application.Features.Weeks.Queries.GetWeeks
{
    public class GetWeeksQuery
    {
        public int? PhaseId { get; set; }
        // null  → toutes les semaines
        // int   → semaines d'une phase précise
    }
}