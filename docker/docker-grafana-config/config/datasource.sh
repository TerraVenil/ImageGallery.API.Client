#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

grafana_datasource_import() {

    grafana_host="http://grafana:3000"
    grafana_cred="admin:admin"
    datasource_name="prom"
    datasource_type="prometheus"
    

    if ! curl --retry 5 --retry-delay 0 -sf http://admin:admin@grafana:3000/api/datasources/name/$datasource_name; then
        
        printf "*** Creating Datasource *** ${GREEN} Name:$datasource_name|Type:$datasource_type ${NC}:\n"

        curl -v -s -k -u "$grafana_cred" -XPOST -H "Accept: application/json" \
            -H "Content-Type: application/json" \
            -d  '{"name":"prom","type":"prometheus","url":"http://prometheus:9090","access":"proxy","isDefault":true}' \
            $grafana_host/api/datasources; echo ""
    fi
}

grafana_datasource_import;

