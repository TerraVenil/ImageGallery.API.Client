FROM appropriate/curl

USER root
COPY config/init.sh .
RUN chmod 755 -c /init.sh


COPY dashboard/ps-monitoring-dashboard.json /var/lib/grafana/dashboards/


ENTRYPOINT ["/init.sh"]