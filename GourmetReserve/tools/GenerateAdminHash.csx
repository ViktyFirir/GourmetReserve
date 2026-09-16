// Мини-утилита для генерации хэша пароля администратора.
// Запуск (из папки проекта, где уже установлен пакет Microsoft.AspNetCore.Identity):
//   dotnet script tools/GenerateAdminHash.csx -- "МойПароль123!"
// либо оформите как отдельную консольную программу:
//
//   var hasher = new PasswordHasher<object>();
//   var hash = hasher.HashPassword(null!, args[0]);
//   Console.WriteLine(hash);
//
// Полученную строку вставьте в appsettings.json -> "AdminUser": { "PasswordHash": "..." }

#r "nuget: Microsoft.AspNetCore.Identity, 2.2.0"
using Microsoft.AspNetCore.Identity;

if (Args.Count == 0)
{
    Console.WriteLine("Использование: dotnet script GenerateAdminHash.csx -- \"ВашПароль\"");
    return;
}

var hasher = new PasswordHasher<object>();
var hash = hasher.HashPassword(null!, Args[0]);
Console.WriteLine(hash);
