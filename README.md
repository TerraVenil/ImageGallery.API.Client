# ImageGallery.API.Client

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/imagegallery-api-client.svg)](https://hub.docker.com/r/stuartshay/imagegallery-api-client/)

[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryAPIClient&metric=alert_status)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryAPIClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryAPIClient&metric=reliability_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryAPIClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryAPIClient&metric=security_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryAPIClient)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=ImageGalleryAPIClient&metric=sqale_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=ImageGalleryAPIClient)

 Jenkins | Status  
------------ | -------------
Build Image (Console) | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-API/ImageGallery-API-Client.Console)](https://jenkins.navigatorglass.com/job/ImageGallery-API/job/ImageGallery-API-Client.Console/)
SonarQube | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=ImageGallery-API/ImageGallery-API-Client-Sonarqube)](https://jenkins.navigatorglass.com/job/ImageGallery-API/job/ImageGallery-API-Client-Sonarqube/)

### Prerequisites

```
.NET Core 2.1
VS Code 1.19.1 or VS 2017 15.8.0
```

### Install

```
cd ImageGallery.API.Client
dotnet restore

cd src\ImageGalleryAPI.Client.Console
dotnet run

```

### Machine Tag Filter

#### Namespace

```
machine_tags => nycparks:
```

#### Predicate

```
machine_tags => nycparks:m010=
https://www.flickr.com/photos/tags/nycparks:m010
```

#### Key
```
machine_tags => nycparks:m010=114
https://www.flickr.com/photos/tags/nycparks:m010=114
```
