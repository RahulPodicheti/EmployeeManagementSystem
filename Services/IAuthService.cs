using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
    }
}