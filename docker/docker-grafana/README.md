# ImageGallery Grafana

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-grafana.svg)](https://hub.docker.com/r/stuartshay/imagegallery-grafana/)


 Jenkins | Status  
------------ | -------------
Build Image  | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-Infrastructure/imagegallery-grafana)](https://jenkins.navigatorglass.com/job/ImageGallery-Infrastructure/job/imagegallery-grafana/)

```
├── grafana                     # Grafana config
│   ├── Dockerfile              # Dockerfile that adds config to the image
│   ├── config.ini              # Base configuration
│   ├── dashboards              # Pre-made dashboards
│   │   └── mydashboard.json    # Sample dashboard
│   └── provisioning            # Configuration for automatic provisioning at
│       │                       # grafana startup.
│       ├── dashboards          
│       │   └── all.yml         # Configuration about grafana dashboard provisioning
│       └── datasources
│           └── all.yml         # Configuration about grafana
```
