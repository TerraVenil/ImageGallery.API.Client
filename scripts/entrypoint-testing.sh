#!/bin/bash
set -e

export PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/opt/mssql-tools/bin

/wait_for_it.sh -h database -p 1433 -t 0 --

until /healthcheck.sh
do
    echo healthcheck
    sleep 5
done

exec dotnet xunit -xml /artifacts/intergration-tests.xml
