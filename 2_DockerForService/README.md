# ДЗ №2. Знакомство с Docker
## Задание:
> 1. Установить docker локально. 
> 2. Завернуть приложение из первого ДЗ в docker (написать Dockerfile).
> 3. Написать программу, запускающую другие в контейнере
(поддержать изоляцию hostname, файловой системы, сети, pid).
Поддержать лимитирование по памяти. Реализовать с нуля без
использования инструментов контейнеризации.

## Установка docker
Так как у меня стоит Debian, то я использовал следующие ресурсы:
  * https://www.8host.com/blog/ustanovka-i-ispolzovanie-docker-v-debian-9/

## Запуск нашего сервиса погоды из TaskAvito1 с помощью Docker
1. Скачиваем репозиторий git clone https://github.com/Vankezzz/TaskAvito2 (установите git предварительно)
2. переходим в папку TaskAvito2
3. Создаем образ с нашим сервисом погоды: docker build -t <my_image> –f <my_dockerfile> . 
>* <my_image> - имя нашего контейнера, допустим weatherservice
>* <my_dockerfile> - имя нашего докер файла, в нашем случае его имя Dockerfile
>* Не забудь точку в конце команды!!!
В итоге: sudo docker build -t weatherservice -f Dockerfile .
4. Запускаем наш контейнер: sudo docker run -it --rm -p 5000:80 -e OWM_API_KEY=2d6d053b6a777cd0fe236dd2c0d9aa22 ---name example1 weatherservice
>* Мы создаем докер
>* -it - инициализация
>* --rm – удаляется контейнер при закрытии
>* -p 5000:80 – мы перетаскивает порт 80 в докере на порт 5000 локального компьютера
>* -e [перем.] – создание переменной окружения 
>* --name [имя_контейнера] – задаем имя 
>* weatherservice  - Имя образа который мы ранее построили
5. Проверяем работоспособность нашего сервиса командой: curl  "http://localhost:5000/v1/current/?city=Moscow" 
> Мы используем http протокол в этом задании так, как с https ( в задании TaskAvito1) много гемороя

## Свой Мини-Докер
Докер создает свой хостнейм и изолирует процесс, а также создает канал интернета
1. Компилируем задачу процесса: gcc hello.c -o hello
2. Компилируем докер: gcc mydocker.c -o my_docker
3. Запускаем докер и он попросит путь до скомпилированного файла hello на 1 шаге : sudo ./net_namespaces

## Источники материала
   * https://rtfm.co.ua/what-is-linux-namespaces-primery-n..
   * https://habr.com/ru/company/selectel/blog/303190/
