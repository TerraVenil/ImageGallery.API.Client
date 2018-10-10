FROM alpine:latest

RUN apk add --update \
    curl \
    jq \
    nodejs \
    nodejs-npm \
&& rm -rf /var/cache/apk/*

RUN npm install -g wizzy


USER root
COPY config/init.sh .
RUN chmod +x -c /init.sh

#CMD ["/init.sh"]
ENTRYPOINT ["/init.sh"]
