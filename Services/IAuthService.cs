using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
        Task RegisterUserAsync( string username, string email, string password, string role);
    }
}