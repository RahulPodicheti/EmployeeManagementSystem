using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task<Employee?> GetEmployeeByIdAsync(int id);

        Task AddEmployeeAsync(Employee employee);

        Task UpdateEmployeeAsync(Employee employee);

        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchText);
        Task<IEnumerable<Employee>> GetEmployeesPagedAsync(int pageNumber, int pageSize);
        Task<int> GetEmployeeCountAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<DashboardStats> GetDashboardStatsAsync();
    }
}