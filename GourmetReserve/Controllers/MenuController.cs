using GourmetReserve.Data;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Menu
        // Возвращает категории вместе с их блюдами (сгруппировано)
        public async Task<IActionResult> Index()
        {
            var categories = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Menu/Category/5
        // Отдельная страница по одной категории (опционально)
        public async Task<IActionResult> Category(int id)
        {
            var category = await _context.MenuCategories
                .Include(c => c.MenuItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}
