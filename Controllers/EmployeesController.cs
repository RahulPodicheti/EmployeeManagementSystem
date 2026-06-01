using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IWebHostEnvironment environment,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchText,
            int page = 1)
        {
            const int pageSize = 5;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchResult =
                    await _employeeService.SearchEmployeesAsync(searchText);

                var searchModel = new EmployeeListViewModel
                {
                    Employees = searchResult,
                    CurrentPage = 1,
                    TotalPages = 1
                };

                ViewBag.SearchText = searchText;

                return View(searchModel);
            }

            var employees =
                await _employeeService.GetEmployeesPagedAsync(
                    page,
                    pageSize);

            var totalEmployees =
                await _employeeService.GetEmployeeCountAsync();

            var model = new EmployeeListViewModel
            {
                Employees = employees,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(
                    totalEmployees / (double)pageSize)
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var departments =
                await _departmentService.GetAllDepartmentsAsync();

            var model = new EmployeeViewModel
            {
                Departments = departments.Select(d =>
                    new SelectListItem
                    {
                        Value = d.DepartmentName,
                        Text = d.DepartmentName
                    })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            EmployeeViewModel model)
        {
            if (await _employeeService.EmailExistsAsync(
                    model.Employee.Email))
            {
                ModelState.AddModelError(
                    "Employee.Email",
                    "Email already exists.");
            }

            if (model.Photo != null)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".webp"
                };

                var extension =
                    Path.GetExtension(model.Photo.FileName)
                        .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "Photo",
                        "Invalid file type. Please upload only JPG, JPEG, PNG, GIF, or WEBP images.");
                }
            }

            if (!ModelState.IsValid)
            {
                var departments =
                    await _departmentService.GetAllDepartmentsAsync();

                model.Departments = departments.Select(d =>
                    new SelectListItem
                    {
                        Value = d.DepartmentName,
                        Text = d.DepartmentName
                    });

                return View(model);
            }

            if (model.Photo != null)
            {
                string uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads");

                Directory.CreateDirectory(
                    uploadsFolder);

                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        model.Photo.FileName);

                string filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await model.Photo.CopyToAsync(stream);

                model.Employee.PhotoPath =
                    "/uploads/" + fileName;
            }

            await _employeeService
                .AddEmployeeAsync(model.Employee);
            _logger.LogInformation("Employee Created: {Email}", model.Employee.Email);
            TempData["Success"] = "Employee created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee =
                await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee =
                await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            var departments =
                await _departmentService.GetAllDepartmentsAsync();

            var model = new EmployeeViewModel
            {
                Employee = employee,

                Departments = departments.Select(d =>
                    new SelectListItem
                    {
                        Value = d.DepartmentName,
                        Text = d.DepartmentName
                    })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            Employee employee,
            IFormFile? photo)
        {
            if (photo != null)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".webp"
                };

                var extension =
                    Path.GetExtension(photo.FileName)
                        .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "",
                        "Invalid file type. Please upload only JPG, JPEG, PNG, GIF, or WEBP images.");

                    return View(employee);
                }

                string uploadsFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads");

                Directory.CreateDirectory(
                    uploadsFolder);

                string fileName =
                    Guid.NewGuid().ToString()
                    + extension;

                string filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create);

                await photo.CopyToAsync(stream);

                employee.PhotoPath =
                    "/uploads/" + fileName;
            }

            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployeeAsync(employee);
                _logger.LogInformation("Employee Updated: {EmployeeId}", employee.EmployeeId);
                TempData["Success"] = "Employee updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee =
                await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["Success"] = "Employee deleted successfully.";
            _logger.LogWarning( "Employee Deleted: {EmployeeId}", id);
            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> ExportToExcel()
        {
            var employees =
                await _employeeService.GetAllEmployeesAsync();

            using var workbook =
                new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "Employee ID";
            worksheet.Cell(1, 2).Value = "First Name";
            worksheet.Cell(1, 3).Value = "Last Name";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "Department";
            worksheet.Cell(1, 7).Value = "Salary";
            worksheet.Cell(1, 8).Value = "Joining Date";
            worksheet.Cell(1, 9).Value = "Status";

            int row = 2;

            foreach (var employee in employees)
            {
                worksheet.Cell(row, 1).Value = employee.EmployeeId;
                worksheet.Cell(row, 2).Value = employee.FirstName;
                worksheet.Cell(row, 3).Value = employee.LastName;
                worksheet.Cell(row, 4).Value = employee.Email;
                worksheet.Cell(row, 5).Value = employee.PhoneNumber;
                worksheet.Cell(row, 6).Value = employee.Department;
                worksheet.Cell(row, 7).Value = employee.Salary;
                worksheet.Cell(row, 8).Value = employee.JoiningDate.ToShortDateString();
                worksheet.Cell(row, 9).Value =
                    employee.IsActive ? "Active" : "Inactive";

                row++;
            }

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            var content =
                stream.ToArray();

            _logger.LogInformation(
                "Employees Exported To Excel");

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Employees.xlsx");
        }
    }
}