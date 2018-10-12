#!/bin/bash
set -e

./wait-for-it.sh -h grafana -p 3000 -t 0 --

exec ./config/init.sh

