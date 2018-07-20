#!/bin/bash
set -e

export PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/opt/mssql-tools/bin

if [  ! -z ${MSSQL_SA_PASSWORD_FILE} ];
then
    export SA_PASSWORD=$(cat $MSSQL_SA_PASSWORD_FILE)
else
    export SA_PASSWORD=${MSSQL_SA_PASSWORD}
fi

/wait_for_it.sh -h database -p 1433 -t 0 --

until /healthcheck.sh
do
    echo healthcheck
    sleep 5
done

exec dotnet NavigatorIdentity.Configuration.IdentityServerDataDB.dll