## Open Zipkin - Docker Compose Stack

https://github.com/openzipkin/docker-zipkin


#### Local Configuration

Modify hosts file     
Windows c:\windows\system32\drivers\etc\hosts

```
192.168.99.100 zipkinserver
```

Run Zipkin Stack
```
docker-compose -f docker-compose.yml pull
docker-compose -f docker-compose.yml up
```

### Zipkin Server/UI

```
http://zipkinserver:9411
```

### mysql

```
database: zipkin
port: 3306

L: zipkin
P: zipkin
```
