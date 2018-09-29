FROM appropriate/curl

USER root
COPY config/init.sh .
RUN chmod 755 -c /init.sh

ENTRYPOINT ["/init.sh"]