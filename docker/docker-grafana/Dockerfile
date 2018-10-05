FROM grafana/grafana:5.2.4

USER root

ADD ./provisioning /etc/grafana/provisioning
ADD ./config.ini /etc/grafana/config.ini
COPY  provisioning/dashboards /var/lib/grafana/dashboards







#USER root
#COPY dashboard/ps-monitoring-dashboard.json /var/lib/grafana/dashboards/

# https://github.com/cirocosta/sample-grafana/blob/master/grafana/Dockerfile