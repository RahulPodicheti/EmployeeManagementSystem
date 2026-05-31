using Dapper;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Data;

namespace EmployeeManagementSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly DapperContext _context;

        public DepartmentService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Department>(
                "sp_GetAllDepartments",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Department>(
                "sp_GetDepartmentById",
                new { DepartmentId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddDepartmentAsync(Department department)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_AddDepartment",
                new
                {
                    department.DepartmentName
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_UpdateDepartment",
                department,
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_DeleteDepartment",
                new { DepartmentId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DepartmentExistsAsync(
            string departmentName,
            int departmentId)
        {
            using var connection = _context.CreateConnection();

            var count = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*)
                  FROM Departments
                  WHERE DepartmentName = @DepartmentName
                  AND DepartmentId <> @DepartmentId",
                new
                {
                    DepartmentName = departmentName,
                    DepartmentId = departmentId
                });

            return count > 0;
        }
    }
}