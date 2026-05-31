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
    }
}