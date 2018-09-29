FROM prom/prometheus:v2.4.2

COPY  config/prometheus.yml  /etc/prometheus/prometheus.yml

USER root
RUN chmod 755 -c /etc/prometheus/prometheus.yml