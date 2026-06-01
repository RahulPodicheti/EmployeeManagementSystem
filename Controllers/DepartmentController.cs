using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var departments =
                await _departmentService.GetAllDepartmentsAsync();

            return View(departments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Department department)
        {
            if (await _departmentService.DepartmentExistsAsync(
                    department.DepartmentName,
                    0))
            {
                ModelState.AddModelError(
                    "DepartmentName",
                    "Department already exists.");
            }

            if (!ModelState.IsValid)
                return View(department);

            await _departmentService.AddDepartmentAsync(department);
            TempData["Success"] = "Department created successfully.";
            _logger.LogInformation(
    "Department Created: {Department}",
    department.DepartmentName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department =
                await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Department department)
        {
            if (await _departmentService.DepartmentExistsAsync(
                    department.DepartmentName,
                    department.DepartmentId))
            {
                ModelState.AddModelError(
                    "DepartmentName",
                    "Department already exists.");
            }

            if (!ModelState.IsValid)
                return View(department);

            await _departmentService.UpdateDepartmentAsync(department);
            TempData["Success"] = "Department updated successfully.";
            _logger.LogInformation(
    "Department Updated: {Department}",
    department.DepartmentName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var department =
                await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            TempData["Success"] = "Department deleted successfully.";
            _logger.LogWarning(
    "Department Deleted: {DepartmentId}",
    id);
            return RedirectToAction(nameof(Index));
        }
    }
}