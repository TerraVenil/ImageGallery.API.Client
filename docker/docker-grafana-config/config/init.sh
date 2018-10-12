#!/bin/bash

set -ex

# create new user - L: viewer P:readonly
curl --retry 5 --retry-delay 0 -sf \
    -X POST -H "Content-Type: application/json" \
    -d '{ "name":"viewer", "email":"viewer@org.com", "login":"viewer",  "password":"readonly" }' \
    http://admin:admin@grafana:3000/api/admin/users 


# set user's home dashboard   
curl \
 -X PUT \
 -H 'Content-Type: application/json' \
 -d '{ "homeDashboardId":1 }' \
 http://viewer:readonly@grafana:3000/api/user/preferences



grafana_datasource_import() {

    grafana_host="http://grafana:3000"
    grafana_cred="admin:admin"
    datasource_name="prom"
    datasource_type="prometheus"
    

    if ! curl --retry 5 --retry-delay 0 -sf http://admin:admin@grafana:3000/api/datasources/name/$datasource_name; then
        curl -v -s -k -u "$grafana_cred" -XPOST -H "Accept: application/json" \
            -H "Content-Type: application/json" \
            -d  '{"name":"prom","type":"prometheus","url":"http://prometheus:9090","access":"proxy","isDefault":true}' \
            $grafana_host/api/datasources; echo ""
    fi
}


# 6239 - Mysql - Prometheus
# 6257 NFS Full - https://github.com/rfrail3/grafana-dashboards
# 1598 - Zipkin / Prometheus
grafana_dashboard_import () {

    grafana_host="http://grafana:3000"
    grafana_cred="admin:admin"
    grafana_datasource="prometheus"
    
    ds=(6239 1598)

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

configure_grafana() {
    grafana_datasource_import
    grafana_dashboard_import
}


configure_grafana
exit 0





