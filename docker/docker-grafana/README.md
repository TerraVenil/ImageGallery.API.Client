# ImageGallery Grafana

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-grafana.svg)](https://hub.docker.com/r/stuartshay/imagegallery-grafana/)


 Jenkins | Status  
------------ | -------------
Build Image  | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Infrastructure/imagegallery-grafana)](https://jenkins.navigatorglass.com/job/ImageGallery-Infrastructure/job/imagegallery-grafana/)

```
├── docker-grafana
│   ├── Dockerfile              # Dockerfile 
│   ├── config.ini              # Grafana config
|   |
│   ├── dashboards              # Grafana dashboards
│   │   └── dashboards.json
|   │
│   └── provisioning
│       ├── dashboards
│       │   └── all.yml         # Configuration dashboard provisioning
│       └── datasources
│           └── all.yml         # Configuration datasources provisioning
|
│   └── scripts
│       └── grafana.sh          # Grafana cli configuration
```
