#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color


# 6239 - Mysql - Prometheus
# 6257 NFS Full -
# 1598 - Zipkin / Prometheus
grafana_dashboard_import () {

    grafana_host="http://grafana:3000"
    grafana_cred="admin:admin"
    grafana_datasource="prometheus"
    
    ds=(6239 1598);

    for d in "${ds[@]}"; do
       printf "*** Importing Dashboard *** ${GREEN}$d${NC}:\n"

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






