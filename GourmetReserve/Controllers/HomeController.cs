using GourmetReserve.Data;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /  или /Home/Index
        public async Task<IActionResult> Index()
        {
            // Топ-3 актуальных новостей, самые свежие сверху
            var latestNews = await _context.NewsItems
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.PublishedAt)
                .Take(3)
                .ToListAsync();

            // 3 случайных блюда от шефа.
            // На уровне БД случайную выборку для PostgreSQL проще всего сделать
            // через EF.Functions.Random() (требует npgsql >= 6) или через
            // выборку в память для небольших таблиц. Здесь — надёжный вариант
            // для небольшого количества позиций в меню.
            var chefChoiceIds = await _context.MenuItems
                .Where(m => m.IsChefChoice)
                .Select(m => m.Id)
                .ToListAsync();

            var random = new Random();
            var randomChefIds = chefChoiceIds
                .OrderBy(_ => random.Next())
                .Take(3)
                .ToList();

            var chefChoiceDishes = await _context.MenuItems
                .Include(m => m.MenuCategory)
                .Where(m => randomChefIds.Contains(m.Id))
                .ToListAsync();

            var viewModel = new HomeIndexViewModel
            {
                LatestNews = latestNews,
                ChefChoiceDishes = chefChoiceDishes
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }

    // Простая ViewModel для главной страницы
    public class HomeIndexViewModel
    {
        public List<NewsItem> LatestNews { get; set; } = new();
        public List<MenuItem> ChefChoiceDishes { get; set; } = new();
    }
}
