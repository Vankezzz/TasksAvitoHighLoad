# ДЗ №5-7

## Общие настройки для домашнего задания
1. Скачиваем репозиторий `git clone https://github.com/Vankezzz/TaskAvito5-7`
2. переходим в папку TaskAvito5-7: `cd TaskAvito5-7`
3. Создаем образ с нашим сервисом погоды: docker build -t <my_image> -f <my_dockerfile> . : `sudo docker build -t weatherservice -f Dockerfile .`
>* <my_image> - имя нашего контейнера, допустим weatherservice
>* <my_dockerfile> - имя нашего докер файла, в нашем случае его имя Dockerfile
>* Не забудь точку в конце команды!!!
4. Запускаем наш первый контейнер: `sudo docker run -it --rm -p 5000:80 -e OWM_API_KEY=2d6d053b6a777cd0fe236dd2c0d9aa22 -e LOCALHOST_REDIS=redisA_master -e PORT_REDIS=6379 --net=redis_net --name example1 weatherservice &`
> Он остановится почему то, поэтому мы перезапустим командой : `sudo docker restart example1`
5. Запускаем наш второй контейнер: `sudo docker run -it --rm -p 5001:80 -e OWM_API_KEY=2d6d053b6a777cd0fe236dd2c0d9aa22 -e LOCALHOST_REDIS=redisA_master -e PORT_REDIS=6379 --net=redis_net --name example2 weatherservice &`
> Он остановится почему то, поэтому мы перезапустим командой : `sudo docker restart example2`

## ДЗ №5 L7 - Балансировщик
### Задание:
> 1. Написать конфигурацию для L7 балансировщика (любого) для сервиса из ДЗ 1. 
> 3. На отдельной виртуальной машине поднять балансировщик и настроить его на проксирование на сервис погоды из ДЗ 1

### Написать конфигурацию для L7 балансировщика (любого) для сервиса из ДЗ 1. 
В файле nginx.cong написан L7 балансировщик между двумя сервисами погоды из ДЗ1
  * и другие ( я честно уже не помню)

### На отдельной виртуальной машине поднять балансировщик и настроить его на проксирование на сервис погоды из ДЗ 1
1. На третьем сервере необходимо установить Nginx  и заменить конфигурационный файл /etc/nginx/nginx.config нашим: ` sudo mv nginx.conf /etc/nginx/`
2. Потом выполнить команду перезапуска Nginx'а: `sudo nginx -s reload`
3. Проверить статус: `sudo service nginx status`
4. Запустить еще два окна терминала для параллельной прослушки портов наших серверов 5000 и 5001 и ввести канал прослушки: `sudo tcpdump -i any port 5000` и `sudo tcpdump -i any port 5001` в соответственных окнах
5. В основном окне проверить работоспособность сервера отправив запрос `curl http://localhost:80/v1/current/?city=Moscow `
6. Посмотреть в двух вспомогательных окнах как происходят запросы и распределяется нагрузка с помощью команды: `for n in {1..10}; do curl http://localhost:80/v1/current/?city=Moscow; done
`

## ДЗ №6 Prometehus + VictoriaMetrics
### Задание:
1. Установка prometehus:
```
```
2.

## ДЗ №7 Redis-Cluster
### Задание:
```
sudo docker run -d --rm --name redisA_master  --net redis_net  -v ${PWD}/redis_cluster/Masters/A_master.conf:/etc/redis/A_master.conf  redis:6.0-alpine redis-server /etc/redis/A_master.conf
sudo docker run -d --rm --name redisB_master  --net redis_net  -v ${PWD}/redis_cluster/Masters/B_master.conf:/etc/redis/B_master.conf  redis:6.0-alpine redis-server /etc/redis/B_master.conf
sudo docker run -d --rm --name redisC_master  --net redis_net  -v ${PWD}/redis_cluster/Masters/C_master.conf:/etc/redis/C_master.conf  redis:6.0-alpine redis-server /etc/redis/C_master.conf
```

```
sudo docker run -d --rm --name redisA_slave  --net redis_net  -v ${PWD}/redis_cluster/Slaves/A_slave.conf:/etc/redis/A_slave.conf  redis:6.0-alpine redis-server /etc/redis/A_slave.conf
sudo docker run -d --rm --name redisB_slave  --net redis_net  -v ${PWD}/redis_cluster/Slaves/B_slave.conf:/etc/redis/B_slave.conf  redis:6.0-alpine redis-server /etc/redis/B_slave.conf
sudo docker run -d --rm --name redisC_slave  --net redis_net  -v ${PWD}/redis_cluster/Slaves/C_slave.conf:/etc/redis/C_slave.conf  redis:6.0-alpine redis-server /etc/redis/C_slave.conf
```

```
sudo docker inspect -f '{{ (index .NetworkSettings.Networks "redis_net").IPAddress }}' redisB_master   
sudo redis-cli --cluster create 172.18.0.2:6379 172.18.0.3:6380 172.18.0.4:6381  172.18.0.5:6382 172.18.0.6:6383 172.18.0.7:6384 --cluster-replicas 1 --verbose --cluster-yes 
redis-cli -c -h 172.18.0.2 -p 6379
CLUSTER NODES
```
### Источники:
1. https://github.com/marcel-dempers/docker-development-youtube-series/tree/master/storage/redis/clustering
