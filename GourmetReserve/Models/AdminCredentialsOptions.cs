namespace GourmetReserve.Models
{
    /// <summary>
    /// Простая конфигурация единственного администратора сайта.
    /// Заполняется из секции "AdminUser" в appsettings.json / переменных окружения.
    /// Для реального проекта с несколькими сотрудниками замените на
    /// ASP.NET Core Identity — эта модель годится как минимальный старт.
    /// </summary>
    public class AdminCredentialsOptions
    {
        public string Username { get; set; } = string.Empty;

        // Хранится только хэш пароля (см. README — раздел "Администратор"),
        // никогда не храните пароль в открытом виде.
        public string PasswordHash { get; set; } = string.Empty;
    }
}
