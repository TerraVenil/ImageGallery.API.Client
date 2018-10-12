FROM alpine:latest

USER root

COPY entrypoint.sh .
COPY config/init.sh  config/init.sh

RUN apk add --update \
    bash \
    curl \
    jq \
    nodejs \
    nodejs-npm \
 && rm -rf /var/cache/apk/* \
 && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
 && chmod +x /*.sh \
 && chmod +x config/*.sh

RUN npm install -g wizzy

#CMD ["/init.sh"]
#ENTRYPOINT ["/init.sh"]

ENTRYPOINT ["/entrypoint.sh"]
