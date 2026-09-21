<div align="center">

# GourmetReserve

**Сайт ресторана с онлайн-бронированием столов**

ASP.NET Core MVC · Entity Framework Core · PostgreSQL · Bootstrap 5 · Vanilla JS

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF%20Core-Code%20First-512BD4?style=flat-square)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat-square&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker&logoColor=white)
![License](https://img.shields.io/badge/license-учебный%20проект-lightgrey?style=flat-square)

</div>

---

## Что внутри

| | |
|---|---|
|  **Главная** | Фото-герой, новости, подборка блюд «выбор шефа» |
|  **Меню** | Категории блюд, пометки «острое / вегетарианское / шеф» |
|  **Бронирование** | Интерактивная карта зала, проверка занятости стола в реальном времени |
|  **Админ-панель** | Вход по логину/паролю, CRUD для меню и новостей |
|  **PostgreSQL** | Все данные — столы, брони, меню, новости — в одной базе, ничего в памяти |

---

##  Стек

- **Backend:** ASP.NET Core MVC (.NET 9)
- **Данные:** Entity Framework Core (Code First) + Npgsql → PostgreSQL 16
- **Frontend:** Razor Views, Bootstrap 5, чистый JavaScript (без фреймворков)
- **Auth:** куки-аутентификация ASP.NET Core (один администратор)
- **Инфраструктура:** Docker Compose (PostgreSQL + Adminer)

---

##  Структура проекта

```
GourmetReserve/
├── Models/              # Table, Reservation, MenuCategory, MenuItem, NewsItem...
├── Data/                # ApplicationDbContext, DbInitializer (seed-данные)
├── Controllers/         # Home, Menu, Booking, Account, Admin
├── Views/                # Razor-представления + общий _Layout
├── wwwroot/
│   ├── css/              # site.css, booking.css
│   ├── js/booking.js     # логика карты зала и AJAX-бронирования
│   └── images/           # фото (герой, блюда, новости — см. README-images.md)
├── tools/GenerateAdminHash.csx
├── docker-compose.yml
├── Program.cs
├── appsettings.json
└── GourmetReserve.csproj
```

---

##  Быстрый старт

### 1. Поднять базу данных

```bash
docker compose up -d
```

Поднимутся два контейнера:
- **postgres** — порт `5432`, БД `gourmet_reserve`, логин/пароль `postgres` / `postgres`
- **adminer** — веб-интерфейс для просмотра базы: [http://localhost:8080](http://localhost:8080) (Система: PostgreSQL, Сервер: `postgres`)

> Уже есть свой PostgreSQL? Просто поменяй `ConnectionStrings:DefaultConnection` в `appsettings.json` и пропусти этот шаг.

### 2. Запустить приложение

```bash
dotnet restore
dotnet run
```

При первом старте `DbInitializer` сам создаст все таблицы (`Database.EnsureCreatedAsync()` — без отдельного `dotnet ef migrations add`) и засеет тестовые данные: 4 стола, 4 категории меню с блюдами, 3 новости.

### 3. Открыть сайт

Адрес будет показан в консоли (обычно `https://localhost:5001`) — Главная, Меню и Бронирование сразу работают на реальных данных из PostgreSQL.

>  Нужно будет менять схему БД после первого запуска? Переходи на настоящие миграции: один раз `dotnet ef migrations add InitialCreate`, замени `EnsureCreatedAsync()` в `DbInitializer.cs` на `MigrateAsync()`.

---

##  Как устроено бронирование

```
GET  /Booking/GetAvailableTables?date=...&time=...
```
Ищет в `Reservations` все брони, чьё время попадает в диапазон **±2 часа** от запрошенного, и возвращает ID занятых столов. Эти ID фронтенд (`booking.js`) блокирует атрибутом `disabled` без перезагрузки страницы.

```
POST /Booking/ConfirmBooking
```
Перед записью в БД сервер **заново** проверяет: (а) гостей не больше вместимости стола, (б) на выбранный интервал нет пересекающейся брони — фронтенду не доверяет. Успешная бронь сохраняется новой строкой в `Reservations`.

---

## Администратор

Куки-аутентификация, один админ-логин (без ASP.NET Core Identity — для нескольких сотрудников/ролей стоит на него перейти).

<details>
<summary><strong>Как задать пароль</strong></summary>

1. Сгенерируй хэш (`PasswordHasher<T>`, как в Identity) — временно вставь перед `app.Run()` в `Program.cs`:
   ```csharp
   var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
   Console.WriteLine(hasher.HashPassword(null!, "МойНадёжныйПароль123!"));
   ```
2. `dotnet run`, скопируй строку из консоли, убери временный код.
   (Тот же код лежит отдельно в `tools/GenerateAdminHash.csx`.)
3. Вставь хэш в `appsettings.json`:
   ```json
   "AdminUser": { "Username": "admin", "PasswordHash": "<строка из шага 1>" }
   ```
4. `/Account/Login` → `/Admin` → `MenuItems` / `NewsItems` — список, создание, редактирование, удаление.

</details>

---

## Картинки

Все нужные фото (герой, галерея на главной, блюда, новости) — с точными именами файлов и подсказками, что искать — в [`wwwroot/images/README-images.md`](wwwroot/images/README-images.md).

---

<div align="center">

Сделано на ASP.NET Core · Данные живут в PostgreSQL, а не в голове разработчика 

</div>
