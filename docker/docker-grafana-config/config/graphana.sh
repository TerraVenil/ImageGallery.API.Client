#!/bin/sh
 
grafana_settings () { 
  curl http://grafana:3000/api/admin/settings   
}





grafana_dashboard_import () {

  dashboard_id=$1
  echo $dashboard_id 

  last_revision=$(curl -sf https://grafana.com/api/dashboards/$dashboard_id/revisions | grep '"revision":' | sed 's/ *"revision": \([0-9]*\),/\1/' | sort -n | tail -1)


  echo ', "inputs": [{"name": "DS_PROMETHEUS", "pluginId": "prometheus", "type": "datasource", "value": "prom"}], "overwrite": true}' >> data_$dashboard_id.json 
  curl --retry 5 --retry-delay 0 -sf \
     -X POST -H "Content-Type: application/json" \
     --data-binary @data_$dashboard_id.json \
     http://grafana:3000/api/dashboards/import

}

grafana_dashboards () { 
  dashboard_id=$1
  echo $dashboard_id 
  curl http://grafana:3000/api/dashboards/id/6239/versions 
}

grafana_dashboard_import 6239
grafana_dashboards 6239

