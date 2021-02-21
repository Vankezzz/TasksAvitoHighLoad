# ДЗ №1. Знакомство с Linux системами. Написание сервиса погоды.
## Задание:
> Написать сервис для отдачи прогноза погоды при условии:
> 1. Данные брать из внешнего API прогноза погоды. 
> 2. URL внешнего API вынести в конфигурационный файл или env переменную.
> 3. Запрос на получение прогноза погоды должен быть представлен в формате JSON:
>*  GET /v1/forecast/?city={city}&dt={timestamp}
>* response:
>*  {
>* "city": "Moscow",
>* "unit": "celsius",
>* "temperature": 32
>* }  
> 4.Запрос на получение текущей погоды должен быть представлен в формате JSON:
>* GET /v1/current/?city={city}
>* response:
>* {
>* "city": "Moscow",
>* "unit": "celsius",
>* "temperature": 25
>* }
> 5. Запустить сервис на Linux



## Инструкция по работе с библиотекой:
1. Скачать репозиторий
2. Установить компилятор для проекта написанного на C# на платформе ASP.NET Core
   * Зайти на сайт https://docs.microsoft.com/ru-ru/dotnet/core/install/linux и пошагово выполнить команды для своего Linux. Ниже приведен пример для Debian 10
     1. Перейдем https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-debian
     2. Установка пакета SDK
        * sudo apt-get update; \\
        * sudo apt-get install -y apt-transport-https && \\
        * sudo apt-get update && \\
        * sudo apt-get install -y dotnet-sdk-3.1
     3. Установка среды выполнения
        * sudo apt-get update; \\
          sudo apt-get install -y apt-transport-https && \\
          sudo apt-get update && \\
          sudo apt-get install -y aspnetcore-runtime-3.1
3. Скомпилировать проект
   * Заходим в директорию проекта TasksAvito и ищем WeatherService.sln и прописываем в консоли: dotnet build WeatherService.sln
4. Создать env переменную OWM_API_KEY={API KEY с сайта https://openweathermap.org/}
   > Можно использовать мой API_KEY=2d6d053b6a777cd0fe236dd2c0d9aa22
   * Для создания env переменной ввести в консоли: export OWM_API_KEY=2d6d053b6a777cd0fe236dd2c0d9aa22
   * Проверить ее существование в списке всех env переменных export
5. Запустить сервис в ассинхронном режиме:
   * Переходим  ~/WeatherService/WeatherService/bin/Debug/netcoreapp3.1 из нашей основной папки TasksAvito
   * запускаем проект командой: dotnet WeatherService.dll &
   * Вывод должен быть аналогичен:
   * ![alt text](https://cdn1.savepice.ru/uploads/2020/10/13/c2772ece92d5016ad0ef7fa95c3a360b-full.jpg)
   * Запоминаем https хост, в данном примере https://localhost:5001
6. Проверяем прогноз погоды по команде: curl -k "https://localhost:5001/v1/forecast/?city={city}&timestamp={timestamp}"
   * {city} - Тут мы вставляем крупный город (Moscow)
   * {timestamp} - Тут мы вставляем время насколько часов вперед прогноз (от 0 до 96)
   * Итоговый пример:curl -k "https://localhost:5001/v1/forecast/?city=Moscow&timestamp=9"
7. Проверяем текущую погоду по команде: curl -k "https://localhost:5001/v1/current/?city={city}"
   * {city} - Тут мы вставляем крупный город (Moscow)
   * Итоговый пример:curl -k "https://localhost:5001/v1/current/?city=Moscow"
