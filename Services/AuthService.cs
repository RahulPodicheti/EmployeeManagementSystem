using Dapper;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Data;

namespace EmployeeManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly DapperContext _context;

        public AuthService(DapperContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(
            string username,
            string password)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_Login",
                new
                {
                    Username = username,
                    Password = password
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UserExistsAsync(
            string username)
        {
            using var connection = _context.CreateConnection();

            var count = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*)
                  FROM Users
                  WHERE Username = @Username",
                new { Username = username });

            return count > 0;
        }

        public async Task RegisterUserAsync(
            string username,
            string email,
            string password,
            string role)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                "sp_RegisterUser",
                new
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    Role = role
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}