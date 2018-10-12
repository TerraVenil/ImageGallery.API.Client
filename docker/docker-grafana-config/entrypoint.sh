#!/bin/bash
set -e

./wait_for_it.sh -h grafana -p 3000 -t 0 --

exec ./config/init.sh

