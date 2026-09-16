using GourmetReserve.Data;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Controllers
{
    [Authorize] // требует куки-аутентификацию, см. AccountController
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        public IActionResult Index()
        {
            return View();
        }

        // ==================== MENU ITEMS ====================

        // GET: /Admin/MenuItems
        public async Task<IActionResult> MenuItems()
        {
            var items = await _context.MenuItems
                .Include(m => m.MenuCategory)
                .OrderBy(m => m.MenuCategory!.SortOrder)
                .ThenBy(m => m.Name)
                .ToListAsync();

            return View(items);
        }

        // GET: /Admin/CreateMenuItem
        public async Task<IActionResult> CreateMenuItem()
        {
            ViewBag.Categories = await _context.MenuCategories.OrderBy(c => c.SortOrder).ToListAsync();
            return View(new MenuItem());
        }

        // POST: /Admin/CreateMenuItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuItem(MenuItem model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.MenuCategories.OrderBy(c => c.SortOrder).ToListAsync();
                return View(model);
            }

            _context.MenuItems.Add(model);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Блюдо добавлено.";
            return RedirectToAction(nameof(MenuItems));
        }

        // GET: /Admin/EditMenuItem/5
        public async Task<IActionResult> EditMenuItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Categories = await _context.MenuCategories.OrderBy(c => c.SortOrder).ToListAsync();
            return View(item);
        }

        // POST: /Admin/EditMenuItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(int id, MenuItem model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.MenuCategories.OrderBy(c => c.SortOrder).ToListAsync();
                return View(model);
            }

            _context.MenuItems.Update(model);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Блюдо обновлено.";
            return RedirectToAction(nameof(MenuItems));
        }

        // POST: /Admin/DeleteMenuItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Блюдо удалено.";
            }

            return RedirectToAction(nameof(MenuItems));
        }

        // ==================== NEWS ITEMS ====================

        // GET: /Admin/NewsItems
        public async Task<IActionResult> NewsItems()
        {
            var news = await _context.NewsItems
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();

            return View(news);
        }

        // GET: /Admin/CreateNewsItem
        public IActionResult CreateNewsItem()
        {
            return View(new NewsItem { PublishedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), IsActive = true });
        }

        // POST: /Admin/CreateNewsItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewsItem(NewsItem model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.NewsItems.Add(model);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Новость опубликована.";
            return RedirectToAction(nameof(NewsItems));
        }

        // GET: /Admin/EditNewsItem/5
        public async Task<IActionResult> EditNewsItem(int id)
        {
            var news = await _context.NewsItems.FindAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        // POST: /Admin/EditNewsItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNewsItem(int id, NewsItem model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.NewsItems.Update(model);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Новость обновлена.";
            return RedirectToAction(nameof(NewsItems));
        }

        // POST: /Admin/DeleteNewsItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsItem(int id)
        {
            var news = await _context.NewsItems.FindAsync(id);
            if (news != null)
            {
                _context.NewsItems.Remove(news);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Новость удалена.";
            }

            return RedirectToAction(nameof(NewsItems));
        }
    }
}
