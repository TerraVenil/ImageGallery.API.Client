FROM alpine:latest

RUN apk add --update curl && rm -rf /var/cache/apk/*

USER root
COPY config/init.sh .
RUN chmod +x -c /init.sh


ENTRYPOINT ["/init.sh"]
