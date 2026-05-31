using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeListViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        = Enumerable.Empty<Employee>();
    public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }

}
