#!/bin/sh
 
set -xeuo pipefail

# create new user - L: viewer P:readonly
curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
    -X POST -H "Content-Type: application/json" \
    -d '{ "name":"viewer", "email":"viewer@org.com", "login":"viewer",  "password":"readonly" }' \
    http://admin:admin@grafana:3000/api/admin/users 


# set user's home dashboard   
curl \
 -X PUT \
 -H 'Content-Type: application/json' \
 -d '{ "homeDashboardId":1 }' \
 http://viewer:readonly@grafana:3000/api/user/preferences



## DataSource 
if ! curl --retry 5 --retry-connrefused --retry-delay 0 -sf http://grafana:3000/api/dashboards/name/prom; then
    curl -sf -X POST -H "Content-Type: application/json" \
         --data-binary '{"name":"prom","type":"prometheus","url":"http://prometheus:9090","access":"proxy","isDefault":true}' \
         http://grafana:3000/api/datasources
fi


## Dashboards - Install 2204
dashboard_id=1598
last_revision=$(curl -sf https://grafana.com/api/dashboards/${dashboard_id}/revisions | grep '"revision":' | sed 's/ *"revision": \([0-9]*\),/\1/' | sort -n | tail -1)

echo '{"dashboard": ' > data.json
curl -s https://grafana.com/api/dashboards/${dashboard_id}/revisions/${last_revision}/download >> data.json
echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "prom"}], "overwrite": false}' >> data.json
curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
     -X POST -H "Content-Type: application/json" \
     --data-binary @data.json \
     http://grafana:3000/api/dashboards/import



dashboard_id_1=6239
last_revision_1=$(curl -sf https://grafana.com/api/dashboards/${dashboard_id_1}/revisions | grep '"revision":' | sed 's/ *"revision": \([0-9]*\),/\1/' | sort -n | tail -1)

echo '{"dashboard": ' > data2.json
curl -s https://grafana.com/api/dashboards/${dashboard_id}/revisions/${last_revision}/download >> data2.json
echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "prom"}], "overwrite": false}' >> data2.json
curl --retry-connrefused --retry 5 --retry-delay 0 -sf \
     -X POST -H "Content-Type: application/json" \
     --data-binary @data2.json \
     http://grafana:3000/api/dashboards/import



