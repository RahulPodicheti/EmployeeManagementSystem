using Dapper;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Data;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DapperContext _context;

        public EmployeeService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Employee>(
                "sp_GetAllEmployees",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Employee>(
                "sp_GetEmployeeById",
                new { EmployeeId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_AddEmployee",
                new
                {
                    employee.FirstName,
                    employee.LastName,
                    employee.Email,
                    employee.PhoneNumber,
                    employee.Department,
                    employee.Salary,
                    employee.JoiningDate,
                    employee.PhotoPath
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_UpdateEmployee",
                new
                {
                    employee.EmployeeId,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email,
                    employee.PhoneNumber,
                    employee.Department,
                    employee.Salary,
                    employee.JoiningDate,
                    employee.PhotoPath
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_DeleteEmployee",
                new { EmployeeId = id },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchText)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Employee>(
                "sp_SearchEmployees",
                new { SearchText = searchText },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<Employee>> GetEmployeesPagedAsync(
    int pageNumber,
    int pageSize)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Employee>(
                "sp_GetEmployeesPaged",
                new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetEmployeeCountAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                "sp_GetEmployeeCount",
                commandType: CommandType.StoredProcedure);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _context.CreateConnection();

            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Employees WHERE Email = @Email",
                new { Email = email });

            return count > 0;
        }
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<DashboardStats>(
                       "sp_GetDashboardStats",
                       commandType: CommandType.StoredProcedure)
                   ?? new DashboardStats();
        }
    }
}