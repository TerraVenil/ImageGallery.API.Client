#!/bin/sh
 

grafana_dashboard_import () {
  
  dashboard_id=$1
  echo dashboard_id $1
  
  last_revision=$(curl -sf https://grafana.com/api/dashboards/${dashboard_id}/revisions | grep '"revision":' | sed 's/ *"revision": \([0-9]*\),/\1/' | sort -n | tail -1)

  echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "prom"}], "overwrite": false}' >> data.json
  curl --retry 5 --retry-delay 0 -sf \
     -X POST -H "Content-Type: application/json" \
     --data-binary @data.json \
     http://grafana:3000/api/dashboards/import

}



grafana_dashboard_import 6239




#grafana_dashboard_import Test2


