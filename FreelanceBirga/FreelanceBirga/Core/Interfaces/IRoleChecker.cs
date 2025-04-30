
namespace FreelanceBirga.Core.Interfaces
{
    public interface IRoleChecker
    {
        Task<bool> UserHasChatAccessAsync(int userId);
    }
}