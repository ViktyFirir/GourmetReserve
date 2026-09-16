# GourmetReserve — полный проект

Готовое ASP.NET Core MVC приложение (.NET 9) + EF Core (Code First) + PostgreSQL
(Npgsql) + Bootstrap 5 + чистый JS. Все данные, включая брони столов
(`Reservations`), хранятся в PostgreSQL через `ApplicationDbContext` — отдельного
хранилища для броней нет, это одна из таблиц той же базы.

## Структура

```
GourmetReserve/
├── Models/
│   ├── Table.cs
│   ├── Reservation.cs
│   ├── MenuCategory.cs
│   ├── MenuItem.cs
│   ├── NewsItem.cs
│   ├── BookingViewModel.cs
│   ├── LoginViewModel.cs
│   └── AdminCredentialsOptions.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── Controllers/
│   ├── HomeController.cs
│   ├── MenuController.cs
│   ├── BookingController.cs
│   ├── AccountController.cs
│   └── AdminController.cs
├── Views/
│   ├── Shared/_Layout.cshtml
│   ├── Home/Index.cshtml
│   ├── Menu/Index.cshtml
│   ├── Booking/Index.cshtml
│   ├── Account/Login.cshtml
│   ├── Admin/ (Index, MenuItems, NewsItems, Create*/Edit* + partial формы)
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/site.css
│   ├── css/booking.css
│   └── js/booking.js
├── tools/GenerateAdminHash.csx
├── docker-compose.yml
├── Program.cs
├── appsettings.json
└── GourmetReserve.csproj
```

## Запуск за 3 шага

1. **Поднять PostgreSQL** (учётные данные уже согласованы с `appsettings.json`):
   ```bash
   docker compose up -d
   ```
   Поднимутся два контейнера: `postgres` (порт 5432, БД `gourmet_reserve`,
   пользователь/пароль `postgres`/`postgres`) и `adminer` — веб-интерфейс для
   просмотра базы на http://localhost:8080 (Система: PostgreSQL, Сервер: `postgres`).

   Если PostgreSQL уже установлен у вас локально/на сервере — просто
   отредактируйте `ConnectionStrings:DefaultConnection` в `appsettings.json`
   под свои параметры и пропустите этот шаг.

2. **Восстановить пакеты и запустить**:
   ```bash
   dotnet restore
   dotnet run
   ```
   При первом старте `Data/DbInitializer.cs` сам создаст все таблицы
   (`Database.EnsureCreatedAsync()` — по текущей модели EF Core, без
   отдельного шага `dotnet ef migrations add`) и наполнит их тестовыми
   данными: 4 стола, 4 категории меню с блюдами, 3 новости.

3. Откройте показанный в консоли адрес (обычно `https://localhost:5001` —
   Главная, Меню, Бронирование сразу работают на реальных данных из БД).

   Если позже понадобится **менять схему** после первого запуска — тогда
   переходите на настоящие миграции: один раз `dotnet ef migrations add
   InitialCreate`, замените `EnsureCreatedAsync()` в `DbInitializer.cs` на
   `MigrateAsync()`, и дальше каждое изменение модели оформляйте новой
   миграцией (`dotnet ef migrations add <Имя>`).

## Логика бронирования (ключевой момент)

- `GET /Booking/GetAvailableTables?date=...&time=...` — ищет в таблице
  `Reservations` (PostgreSQL) все брони, чьё время попадает в диапазон
  `[запрошенное_время − 2ч, запрошенное_время + 2ч]`, и возвращает ID
  занятых столов JSON-массивом. Именно эти ID фронтенд (`booking.js`)
  блокирует атрибутом `disabled`.
- `POST /Booking/ConfirmBooking` — перед записью в БД заново проверяет
  на сервере (a) что гостей не больше вместимости стола и (b) что на
  выбранный интервал нет пересекающейся брони — не доверяет клиенту.
  Успешная бронь сохраняется как новая строка в `Reservations`.

## Администратор (аутентификация)

Куки-аутентификация ASP.NET Core, один администратор (логин/хэш пароля —
в `appsettings.json`, секция `AdminUser`). Достаточно для управления меню
и новостями через `/Admin`. Для нескольких сотрудников/ролей — замените
на `Microsoft.AspNetCore.Identity`.

Чтобы задать пароль:

1. Сгенерируйте хэш (тот же `PasswordHasher<T>`, что в Identity) — временно
   добавьте перед `app.Run()` в `Program.cs`:
   ```csharp
   var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
   Console.WriteLine(hasher.HashPassword(null!, "МойНадёжныйПароль123!"));
   ```
   Запустите `dotnet run`, скопируйте выведенную строку, затем уберите этот код.
   (Тот же код — отдельным скриптом в `tools/GenerateAdminHash.csx`.)
2. Вставьте хэш в `appsettings.json`:
   ```json
   "AdminUser": { "Username": "admin", "PasswordHash": "<строка из шага 1>" }
   ```
3. `/Account/Login` → `/Admin` → `MenuItems` / `NewsItems` (список, создание,
   редактирование, удаление).

## Что осознанно не реализовано (не запрашивалось)
- Полноценный `ASP.NET Core Identity` (роли, несколько пользователей) —
  сейчас один админ-логин.
- Клиентская валидация форм (`jquery.validate.unobtrusive`) — сейчас только
  серверная (Data Annotations); UX без неё чуть менее отзывчивый на форме.
- DB-level exclusion constraint против пересечения броней в PostgreSQL —
  сейчас пересечение проверяется в коде (`BookingController`), а не на
  уровне схемы БД.
