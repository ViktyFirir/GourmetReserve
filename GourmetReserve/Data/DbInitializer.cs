using GourmetReserve.Models;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Data
{
    /// <summary>
    /// Создаёт схему БД и наполняет её тестовыми данными при первом запуске.
    /// Вызывается один раз из Program.cs.
    ///
    /// Используется Database.EnsureCreatedAsync() — она создаёт все таблицы
    /// прямо из текущей модели EF Core (Table, Reservation, MenuCategory,
    /// MenuItem, NewsItem) без отдельного шага "dotnet ef migrations add":
    /// поднимаете PostgreSQL (см. docker-compose.yml) и сразу делаете `dotnet run`.
    ///
    /// Если в будущем понадобится ИЗМЕНЯТЬ схему после первого запуска (добавить
    /// колонку и т.п.) — тогда стоит перейти на полноценные миграции:
    /// один раз выполнить `dotnet ef migrations add InitialCreate`,
    /// заменить вызов ниже на `Database.MigrateAsync()` и далее создавать
    /// миграцию под каждое изменение модели через `dotnet ef migrations add`.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Создаёт БД и все таблицы, если их ещё нет (ничего не делает, если уже есть)
            await context.Database.EnsureCreatedAsync();

            await SeedTablesAsync(context);
            await SeedMenuAsync(context);
            await SeedNewsAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedTablesAsync(ApplicationDbContext context)
        {
            if (await context.Tables.AnyAsync()) return;

            var tables = new List<Table>
            {
                new() { TableNumber = 1, Capacity = 2, Location = TableLocation.Window,   Status = TableStatus.Available, GridRow = 1, GridColumn = 1 },
                new() { TableNumber = 2, Capacity = 4, Location = TableLocation.MainHall,  Status = TableStatus.Available, GridRow = 1, GridColumn = 3 },
                new() { TableNumber = 3, Capacity = 6, Location = TableLocation.Terrace,   Status = TableStatus.Available, GridRow = 2, GridColumn = 2 },
                new() { TableNumber = 4, Capacity = 2, Location = TableLocation.Vip,       Status = TableStatus.Available, GridRow = 3, GridColumn = 4 },
            };

            await context.Tables.AddRangeAsync(tables);
        }

        private static async Task SeedMenuAsync(ApplicationDbContext context)
        {
            if (await context.MenuCategories.AnyAsync()) return;

            var appetizers = new MenuCategory { Name = "Закуски", IconCssClass = "bi bi-egg-fried", SortOrder = 1 };
            var mains = new MenuCategory { Name = "Горячее", IconCssClass = "bi bi-fire", SortOrder = 2 };
            var desserts = new MenuCategory { Name = "Десерты", IconCssClass = "bi bi-cake2", SortOrder = 3 };
            var drinks = new MenuCategory { Name = "Напитки", IconCssClass = "bi bi-cup-straw", SortOrder = 4 };

            await context.MenuCategories.AddRangeAsync(appetizers, mains, desserts, drinks);

            var items = new List<MenuItem>
            {
                new() { MenuCategory = appetizers, Name = "Тартар из тунца", Description = "Тунец, авокадо, соус понзу", Price = 890, WeightOrVolume = "150 г", IsChefChoice = true },
                new() { MenuCategory = appetizers, Name = "Брускетты с томатами", Description = "Чиабатта, томаты, базилик", Price = 450, WeightOrVolume = "180 г", IsVegetarian = true },
                new() { MenuCategory = mains,      Name = "Стейк рибай", Description = "Мраморная говядина, овощи гриль", Price = 2450, WeightOrVolume = "300 г", IsChefChoice = true },
                new() { MenuCategory = mains,      Name = "Том-ям с креветками", Description = "Острый тайский суп", Price = 690, WeightOrVolume = "350 мл", IsSpicy = true },
                new() { MenuCategory = desserts,   Name = "Чизкейк Нью-Йорк", Description = "Классический чизкейк с ягодным соусом", Price = 420, WeightOrVolume = "140 г", IsChefChoice = true },
                new() { MenuCategory = drinks,     Name = "Лимонад домашний", Description = "Мята, лайм, имбирь", Price = 350, WeightOrVolume = "400 мл", IsVegetarian = true },
            };

            await context.MenuItems.AddRangeAsync(items);
        }

        private static async Task SeedNewsAsync(ApplicationDbContext context)
        {
            if (await context.NewsItems.AnyAsync()) return;

            var news = new List<NewsItem>
            {
                new()
                {
                    Title = "Обновили сезонное меню",
                    ShortDescription = "Добавили блюда с осенними трюфелями и тыквой.",
                    FullText = "В новом сезонном меню — блюда с трюфелями, тыквой и другими сезонными продуктами.",
                    PublishedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-1), DateTimeKind.Unspecified),
                    IsActive = true
                },
                new()
                {
                    Title = "Новая терраса открыта",
                    ShortDescription = "Открыли летнюю террасу с видом на набережную.",
                    FullText = "Терраса рассчитана на 20 гостей и работает до конца сезона.",
                    PublishedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-5), DateTimeKind.Unspecified),
                    IsActive = true
                },
                new()
                {
                    Title = "Дегустационный ужин с шефом",
                    ShortDescription = "Специальное событие в конце месяца — 6 блюд от шефа.",
                    FullText = "Регистрация по телефону ресторана, количество мест ограничено.",
                    PublishedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-10), DateTimeKind.Unspecified),
                    IsActive = true
                }
            };

            await context.NewsItems.AddRangeAsync(news);
        }
    }
}
