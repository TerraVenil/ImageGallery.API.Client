#!/bin/sh
set -eux

echo "*** Installing MySQL client"
apk add --update --no-cache mysql-client
chown -R mysql /mysql

echo "*** Starting MySQL"
mysqld --user=mysql --basedir=/usr/ --datadir=/mysql/data &

timeout=300
while [[ "$timeout" -gt 0 ]] && ! mysql --user=mysql --protocol=socket -uroot -e 'SELECT 1' >/dev/null 2>/dev/null; do
    echo "Waiting ${timeout} seconds for mysql to come up"
    sleep 2
    timeout=$(($timeout - 2))
done

echo "*** Importing Schema"
curl https://raw.githubusercontent.com/openzipkin/zipkin/$ZIPKIN_VERSION/zipkin-storage/mysql-v1/src/main/resources/mysql.sql > /mysql/zipkin.sql
mysql --verbose --user=mysql --protocol=socket -uroot <<-EOSQL
USE mysql ;

SELECT User, Host, Password FROM mysql.user; 


DELETE FROM mysql.user ;
DROP DATABASE IF EXISTS test ;

SET GLOBAL innodb_file_format=Barracuda ;
CREATE DATABASE zipkin ;

USE zipkin;
SOURCE /mysql/zipkin.sql ;

GRANT ALL PRIVILEGES ON zipkin.* TO zipkin@'%' IDENTIFIED BY 'zipkin' WITH GRANT OPTION ;
SHOW GRANTS FOR zipkin@'%';


CREATE DATABASE diagnostics ;
USE diagnostics;
GRANT ALL PRIVILEGES ON diagnostics.* TO diagnostics@'%' IDENTIFIED BY 'password' WITH GRANT OPTION ;
SHOW GRANTS FOR diagnostics@'%';

USE mysql ;





SHOW DATABASES;
SELECT User, Host, Password FROM mysql.user; 



FLUSH PRIVILEGES ;
EOSQL

echo "*** Stopping MySQL"
pkill -f mysqld

echo "*** Cleaning Up"
apk del mysql-client --purge