// booking.js — логика интерактивной карты зала и бронирования стола
// Чистый JavaScript, без внешних библиотек.

document.addEventListener('DOMContentLoaded', function () {
    const dateInput = document.getElementById('bookingDate');
    const timeInput = document.getElementById('bookingTime');
    const guestsInput = document.getElementById('guestsCount');
    const tableButtons = document.querySelectorAll('.table-item');
    const selectedTableIdInput = document.getElementById('selectedTableId');
    const confirmBtn = document.getElementById('confirmBookingBtn');
    const selectionStatus = document.getElementById('selectionStatus');
    const bookingResult = document.getElementById('bookingResult');
    const bookingForm = document.getElementById('bookingForm');

    let selectedTableButton = null;

    // ---------- 1. Проверка доступности столов при смене даты/времени ----------
    async function refreshAvailability() {
        const date = dateInput.value;
        const time = timeInput.value;

        // Пока не выбраны и дата, и время — не блокируем столы
        if (!date || !time) {
            return;
        }

        try {
            const url = `/Booking/GetAvailableTables?date=${encodeURIComponent(date)}&time=${encodeURIComponent(time)}`;
            const response = await fetch(url, { method: 'GET' });

            if (!response.ok) {
                console.error('Не удалось получить занятые столы:', response.status);
                return;
            }

            const occupiedTableIds = await response.json(); // массив int

            tableButtons.forEach(function (btn) {
                const tableId = parseInt(btn.dataset.tableId, 10);
                const isOccupied = occupiedTableIds.includes(tableId);

                btn.disabled = isOccupied;
                btn.classList.toggle('table-item--busy', isOccupied);

                // Если ранее выбранный стол вдруг стал занят — сбрасываем выбор
                if (isOccupied && btn.classList.contains('selected')) {
                    clearSelection();
                }
            });
        } catch (err) {
            console.error('Ошибка запроса доступности столов:', err);
        }
    }

    dateInput.addEventListener('change', refreshAvailability);
    timeInput.addEventListener('change', refreshAvailability);

    // ---------- 2. Выбор стола на карте зала ----------
    tableButtons.forEach(function (btn) {
        btn.addEventListener('click', function () {
            if (btn.disabled) {
                return;
            }

            // Проверка вместимости стола относительно указанного числа гостей
            const capacity = parseInt(btn.dataset.capacity, 10);
            const guests = parseInt(guestsInput.value, 10) || 1;

            if (guests > capacity) {
                selectionStatus.textContent =
                    `Этот стол вмещает максимум ${capacity} гостей. Уменьшите число гостей или выберите другой стол.`;
                selectionStatus.classList.add('text-danger');
                return;
            }

            selectionStatus.classList.remove('text-danger');

            // Снимаем выделение с предыдущего стола
            if (selectedTableButton) {
                selectedTableButton.classList.remove('selected');
            }

            btn.classList.add('selected');
            selectedTableButton = btn;
            selectedTableIdInput.value = btn.dataset.tableId;

            selectionStatus.textContent = `Выбран стол №${btn.querySelector('.table-label').textContent.replace('№', '')}`;
            confirmBtn.disabled = false;
        });
    });

    function clearSelection() {
        if (selectedTableButton) {
            selectedTableButton.classList.remove('selected');
        }
        selectedTableButton = null;
        selectedTableIdInput.value = '';
        confirmBtn.disabled = true;
        selectionStatus.textContent = 'Выберите дату, время и стол ниже';
    }

    // ---------- 3. Отправка формы бронирования ----------
    confirmBtn.addEventListener('click', async function () {
        if (!selectedTableIdInput.value) {
            return;
        }

        const formData = new FormData(bookingForm);
        const payload = {
            TableId: parseInt(formData.get('TableId'), 10),
            Date: formData.get('Date'),
            Time: formData.get('Time'),
            GuestName: formData.get('GuestName'),
            Phone: formData.get('Phone'),
            Email: formData.get('Email'),
            GuestsCount: parseInt(formData.get('GuestsCount'), 10),
            SpecialRequests: formData.get('SpecialRequests')
        };

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        confirmBtn.disabled = true;
        confirmBtn.textContent = 'Отправка...';

        try {
            const response = await fetch('/Booking/ConfirmBooking', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify(payload)
            });

            const data = await response.json();

            if (response.ok && data.success) {
                bookingResult.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
                clearSelection();
                bookingForm.reset();
                refreshAvailability();
            } else {
                const errorMessage = data.title || 'Не удалось выполнить бронирование. Проверьте данные и попробуйте снова.';
                bookingResult.innerHTML = `<div class="alert alert-danger">${errorMessage}</div>`;
                confirmBtn.disabled = false;
            }
        } catch (err) {
            console.error('Ошибка при отправке брони:', err);
            bookingResult.innerHTML = `<div class="alert alert-danger">Произошла ошибка сети. Попробуйте позже.</div>`;
            confirmBtn.disabled = false;
        } finally {
            confirmBtn.textContent = 'Подтвердить бронирование';
        }
    });
});
