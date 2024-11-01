FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env

RUN mkdir -p /opt/cert
COPY ./secrets/*.jks /opt/cert
COPY *.pem /opt/cert
COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
COPY --from=installer-env ["/opt/cert", "/opt/cert"]