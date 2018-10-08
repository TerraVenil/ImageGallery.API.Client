#!/bin/bash

# 6239 - Mysql - Prometheus
# 6257 NFS Full - https://github.com/rfrail3/grafana-dashboards
# 1598 - Zipkin / Prometheus
grafana_dashboard_import () {

    grafana_host="http://grafana:3000"
    grafana_cred="admin:admin"
    grafana_datasource="prometheus"
    
    ds=(6239 1598);

    for d in "${ds[@]}"; do
       echo -e "Processing $d:"

        j=$(curl -s -k -u "$grafana_cred" $grafana_host/api/gnet/dashboards/$d | jq .json)
        
        curl -s -k -u "$grafana_cred" -XPOST -H "Accept: application/json" \
            -H "Content-Type: application/json" \
            -d "{\"dashboard\":$j,\"overwrite\":true, \
                \"inputs\":[{\"name\":\"DS_PROMETHEUS\",\"type\":\"datasource\", \
                \"pluginId\":\"prometheus\",\"value\":\"$grafana_datasource\"}]}" \
            $grafana_host/api/dashboards/import; echo ""

    done
}


grafana_dashboard_import;






