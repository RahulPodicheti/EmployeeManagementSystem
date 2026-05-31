using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Department { get; set; }

        public decimal Salary { get; set; }

        public DateTime JoiningDate { get; set; }

        public string? PhotoPath { get; set; }
        public bool IsActive { get; set; } = true;
    }
}