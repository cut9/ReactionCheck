# ReactionCheck (Console)

Приложение для проверки реакции: пользователю предлагается реагировать на событие, измеряется время отклика и выводится в консоль статистика.

[![.NET CI](https://github.com/cut9/ReactionCheck/actions/workflows/dotnet-ci-windows.yml/badge.svg)](https://github.com/cut9/ReactionCheck/actions)
[![Release](https://img.shields.io/github/v/release/cut9/ReactionCheck)](https://github.com/cut9/ReactionCheck/releases)

---

## Информация
- Платформа: кроссплатформенная консоль (.NET)
- Язык: C# .NET 8
- Тип: Консольное приложение (CLI)

---

## Быстрый старт — запустить релиз (рекомендуется для обычных пользователей)

1. Перейдите в раздел **Releases**: https://github.com/cut9/ReactionCheck/releases  
2. Скачайте архив `win-x64.rar` для вашей платформы.  
3. Распакуйте архив и дважды кликните `ReactionCheck.exe`.

---

## Быстрый старт — запустить из исходников (для разработчиков)

### Команды (PowerShell / CMD)
```powershell
git clone https://github.com/cut9/ReactionCheck.git
cd ReactionCheck
dotnet restore
dotnet run --project ./ReactionCheck/ReactionCheck.csproj
```
