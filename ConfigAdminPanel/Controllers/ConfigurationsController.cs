using Config.Abstractions;
using ConfigAdminPanel.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigAdminPanel.Controllers
{
    public class ConfigurationsController : Controller
    {
        private readonly IConfigRepository _repo;
        public ConfigurationsController(IConfigRepository repo) => _repo = repo;

        // LIST
        public async Task<IActionResult> Index(string? app, bool? active)
        {
            var items = await _repo.ListAsync(app, active, HttpContext.RequestAborted);
            ViewBag.App = app;
            ViewBag.Active = active;
            return View(items);
        }

        // CREATE
        [HttpGet]
        public IActionResult Create() => View(new ConfigItemInput());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfigItemInput m)
        {
            if (!ModelState.IsValid) return View(m);

            await _repo.CreateAsync(new ConfigItem(
                0, m.Name, m.Type, m.Value, m.IsActive, m.ApplicationName, DateTime.UtcNow
            ), HttpContext.RequestAborted);

            return RedirectToAction(nameof(Index), new { app = m.ApplicationName });
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id, HttpContext.RequestAborted);
            if (item is null) return NotFound();

            var vm = new ConfigItemInput
            {
                Id = item.Id,
                ApplicationName = item.ApplicationName,
                Name = item.Name,
                Type = item.Type,
                Value = item.Value,
                IsActive = item.IsActive
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConfigItemInput m)
        {
            if (id != m.Id) return BadRequest();
            if (!ModelState.IsValid) return View(m);

            var ok = await _repo.UpdateAsync(new ConfigItem(
                m.Id, m.Name, m.Type, m.Value, m.IsActive, m.ApplicationName, DateTime.UtcNow
            ), HttpContext.RequestAborted);

            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index), new { app = m.ApplicationName });
        }

    }
}
