## Open Zipkin - Docker Compose Stack

https://github.com/openzipkin/docker-zipkin

```
docker-compose -f docker-compose.yml pull
docker-compose -f docker-compose.yml up
```

### Zipkin UI

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

