using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}