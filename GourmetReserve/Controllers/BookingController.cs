using GourmetReserve.Data;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Буфер" вокруг каждой брони: считаем стол занятым в пределах
        // +/- 2 часов от времени уже существующей брони на этот стол.
        private static readonly TimeSpan BookingBuffer = TimeSpan.FromHours(2);

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Booking
        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables
                .Where(t => t.Status == TableStatus.Available)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();

            return View(tables);
        }

        // GET: /Booking/GetAvailableTables?date=2026-09-20&time=19:00
        // Возвращает JSON-массив ID столов, которые ЗАНЯТЫ на указанные дату/время
        // (с учётом пересечения +/- 2 часа с уже существующими бронями).
        [HttpGet]
        public async Task<IActionResult> GetAvailableTables(DateTime date, TimeSpan time)
        {
            var requestedDateTime = date.Date + time;
            var rangeStart = requestedDateTime - BookingBuffer;
            var rangeEnd = requestedDateTime + BookingBuffer;

            // Ищем все брони, чьё время попадает в диапазон [rangeStart, rangeEnd]
            var occupiedTableIds = await _context.Reservations
                .Where(r => r.ReservationDateTime >= rangeStart
                            && r.ReservationDateTime <= rangeEnd)
                .Select(r => r.TableId)
                .Distinct()
                .ToListAsync();

            return Json(occupiedTableIds);
        }

        // POST: /Booking/ConfirmBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking([FromBody] BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var table = await _context.Tables.FindAsync(model.TableId);
            if (table == null)
            {
                ModelState.AddModelError(string.Empty, "Выбранный стол не найден.");
                return BadRequest(ModelState);
            }

            // Повторная серверная проверка: вместимость стола
            if (model.GuestsCount > table.Capacity)
            {
                ModelState.AddModelError(nameof(model.GuestsCount),
                    $"Стол №{table.TableNumber} рассчитан максимум на {table.Capacity} гостей.");
                return BadRequest(ModelState);
            }

            var requestedDateTime = model.GetReservationDateTime();
            var rangeStart = requestedDateTime - BookingBuffer;
            var rangeEnd = requestedDateTime + BookingBuffer;

            // Повторная серверная проверка: доступность стола на выбранное время
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.TableId == model.TableId
                               && r.ReservationDateTime >= rangeStart
                               && r.ReservationDateTime <= rangeEnd);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty,
                    "К сожалению, этот стол уже забронирован на выбранное время. Пожалуйста, выберите другой стол или время.");
                return BadRequest(ModelState);
            }

            var reservation = new Reservation
            {
                GuestName = model.GuestName,
                Phone = model.Phone,
                Email = model.Email,
                TableId = model.TableId,
                ReservationDateTime = requestedDateTime,
                GuestsCount = model.GuestsCount,
                SpecialRequests = model.SpecialRequests,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Стол успешно забронирован!",
                reservationId = reservation.Id
            });
        }
    }
}