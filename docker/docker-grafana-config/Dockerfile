FROM appropriate/curl

USER root
COPY config/init.sh .
RUN chmod +x -c /init.sh


ENTRYPOINT ["/init.sh"]