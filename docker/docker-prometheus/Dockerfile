FROM prom/prometheus:v2.4.2

COPY  config/prometheus.yml  /etc/prometheus/prometheus.yml

# Set Write Permission to override at runtime
USER root
RUN chmod 644 -c /etc/prometheus/prometheus.yml
