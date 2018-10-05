## Open Zipkin/Grafana - Docker Compose Stack

https://github.com/openzipkin/docker-zipkin


#### Local Configuration

Modify hosts file     
Windows c:\windows\system32\drivers\etc\hosts

```
<DOCKER_HOST_IP> zipkin
<DOCKER_HOST_IP> grafana
<DOCKER_HOST_IP> prometheus
<DOCKER_HOST_IP> mysql
```

Run Zipkin/Grafana Stack
```
docker-compose -f docker-compose.yml pull
docker-compose -f docker-compose.yml up
```

Run Zipkin/Grafana     
w/ImageGallery.API.Client.WebApi Stack

```
docker-compose -f docker-compose.yml -f docker-compose-api.yml pull
docker-compose -f docker-compose.yml -f docker-compose-api.yml up
```

### Zipkin Server/UI

```
http://zipkin:9411
```

### Grafana UI

```
http://grafana:3000
```

### Prometheus

```
http://prometheus:9090
```

### mysql

```
database: zipkin
port: 3306

L: zipkin
P: zipkin
```
