FROM microsoft/dotnet:2.1-sdk AS build-env

RUN dotnet --version
RUN cat /etc/os-release

ARG BUILD_NUMBER=0
ENV BUILD_NUMBER ${BUILD_NUMBER}

#COPY src /app/src
#COPY scripts /
#COPY NavigatorIdentity.Configuration.sln /app/NavigatorIdentity.Configuration.sln
#COPY NuGet.config /app/NuGet.config

#WORKDIR /app
#RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

#WORKDIR /app/src/NavigatorIdentity.Configuration.IdentityServerDataDB

#RUN dotnet setversion 2.1.0.${BUILD_NUMBER};
#RUN dotnet publish -o /publish -c Release -f netcoreapp2.1 -r debian.9-x64

# Runtime Image Stage
#FROM microsoft/dotnet:2.1-runtime

#RUN cat /etc/os-release

#COPY --from=build-env /*.sh /

#RUN apt-get update && apt-get install -y \
#        curl \
#        apt-transport-https \
#        gnupg2 \
#        gnupg1 \
#    && rm -rf /var/lib/apt/lists/* \
#    && curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /wait_for_it.sh \
#    && chmod +x /*.sh

## SQL Server Tools
#ENV DEBIAN_VERSION=9

#RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
#    && curl https://packages.microsoft.com/config/debian/${DEBIAN_VERSION}/prod.list > /etc/apt/sources.list.d/mssql-release.list \
#    && apt-get update \
#    && ACCEPT_EULA=Y apt-get install -y mssql-tools

#RUN apt-get install -y locales \
#    && echo "en_US.UTF-8 UTF-8" > /etc/locale.gen \
#    && locale-gen


#ENV PATH="/opt/mssql-tools/bin:${PATH}"

#WORKDIR /publish
#COPY --from=build-env /publish .

#ENTRYPOINT ["/entrypoint.sh"]
