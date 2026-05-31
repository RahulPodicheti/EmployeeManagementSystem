using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }
            = new Employee();

        public IEnumerable<SelectListItem> Departments { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IFormFile? Photo { get; set; }
    }
}