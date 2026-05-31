using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IWebHostEnvironment _environment;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _environment = environment;
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

            return View(employee);
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

            return RedirectToAction(nameof(Index));
        }
    }
}