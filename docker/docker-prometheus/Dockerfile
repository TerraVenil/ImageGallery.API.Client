FROM prom/prometheus:v2.4.2

COPY  config/prometheus.yml  /etc/prometheus/prometheus.yml

# Set Write Permission to override at runtime
USER root
RUN chmod 755 -c /etc/prometheus/prometheus.yml


RUN chmod +x /usr/local/bin/grafana.sh && \
    apt-get update && \
    apt-get install --no-install-recommends less && \
    rm -rf /var/lib/apt/lists/* 
