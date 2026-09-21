# GourmetReserve — полный проект

Готовое ASP.NET Core MVC приложение (.NET 9) + EF Core (Code First) + PostgreSQL
(Npgsql) + Bootstrap 5 + чистый JS. Все данные, включая брони столов
(`Reservations`), хранятся в PostgreSQL через `ApplicationDbContext` — отдельного
хранилища для броней нет, это одна из таблиц той же базы.



## Запуск за 3 шага

1. **Поднять PostgreSQL** (учётные данные уже согласованы с `appsettings.json`):
   ```bash
   docker compose up -d
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
и новостями через `/Admin`. 


