using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoMVC.Data;
using ToDoMVC.Models;

namespace ToDoMVC.Controllers
{
    public class TasksController(ApplicationDbContext dbContext) : Controller
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        [HttpGet("")]
        public async Task<IActionResult> GetAll(string sortOrder, string searchString)
        {
            ViewBag.NameSortOrder = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSearchString = searchString;
            ViewBag.StatusSortOrder = sortOrder == "status_asc" ? "status_desc" : "status_asc";

            var tasks = string.IsNullOrEmpty(searchString) ? _dbContext.TaskItems : _dbContext.TaskItems.Where(t => t.Name.ToUpper().Contains(searchString));

            tasks = sortOrder switch
            {
                "name_desc" => tasks.OrderByDescending(t => t.Name),
                "status_asc" => tasks.OrderBy(t => t.IsCompleted),
                "status_desc" => tasks.OrderByDescending(t => t.IsCompleted),
                _ => tasks.OrderBy(t => t.Name),
            };

            return View(await tasks.ToListAsync());
        }

        [HttpGet("/new")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("/new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Text")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.AddAsync(taskItem);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(GetAll));
            }

            return View(taskItem);
        }

        [HttpGet("/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var taskItem = await _dbContext.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        [HttpPost("/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Text,IsCompleted")] TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedTaskItem = await _dbContext.TaskItems.FindAsync(taskItem.Id);
                _dbContext.Entry(updatedTaskItem).CurrentValues.SetValues(taskItem);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(GetAll));
            }

            return View(taskItem);
        }

        [HttpGet("/{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskItem = await _dbContext.TaskItems.FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        [HttpPost("/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _dbContext.TaskItems.FindAsync(id);
            _dbContext.TaskItems.Remove(taskItem);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(GetAll));
        }

        [HttpPost("/{id}/togglestatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var taskItem = await _dbContext.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            var updatedTaskItem = await _dbContext.TaskItems.FindAsync(id);
            taskItem.IsCompleted = !taskItem.IsCompleted;
            _dbContext.Entry(updatedTaskItem).CurrentValues.SetValues(taskItem);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(GetAll));
        }
    }
}
