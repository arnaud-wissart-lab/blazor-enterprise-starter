FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ["src/BlazorEnterpriseStarter.Server/BlazorEnterpriseStarter.Server.csproj", "src/BlazorEnterpriseStarter.Server/"]
COPY ["src/BlazorEnterpriseStarter.ServiceDefaults/BlazorEnterpriseStarter.ServiceDefaults.csproj", "src/BlazorEnterpriseStarter.ServiceDefaults/"]
COPY ["src/BlazorEnterpriseStarter.Shared/BlazorEnterpriseStarter.Shared.csproj", "src/BlazorEnterpriseStarter.Shared/"]

RUN dotnet restore "src/BlazorEnterpriseStarter.Server/BlazorEnterpriseStarter.Server.csproj"

COPY . .

RUN dotnet publish "src/BlazorEnterpriseStarter.Server/BlazorEnterpriseStarter.Server.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BlazorEnterpriseStarter.Server.dll"]
