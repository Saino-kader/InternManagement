namespace InterManagement.Application.Common
{
    public interface IActivityLogger
    {
        Task LogAsync(string userName, string action, string detail);
    }
}
