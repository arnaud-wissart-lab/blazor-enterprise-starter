FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ["src/BlazorEnterpriseStarter.App/BlazorEnterpriseStarter.App.csproj", "src/BlazorEnterpriseStarter.App/"]
COPY ["src/BlazorEnterpriseStarter.ServiceDefaults/BlazorEnterpriseStarter.ServiceDefaults.csproj", "src/BlazorEnterpriseStarter.ServiceDefaults/"]
COPY ["src/BlazorEnterpriseStarter.Shared/BlazorEnterpriseStarter.Shared.csproj", "src/BlazorEnterpriseStarter.Shared/"]
COPY ["src/BlazorEnterpriseStarter.Components/BlazorEnterpriseStarter.Components.csproj", "src/BlazorEnterpriseStarter.Components/"]
COPY ["src/BlazorEnterpriseStarter.DesignSystem/BlazorEnterpriseStarter.DesignSystem.csproj", "src/BlazorEnterpriseStarter.DesignSystem/"]

RUN dotnet restore "src/BlazorEnterpriseStarter.App/BlazorEnterpriseStarter.App.csproj"

COPY . .

RUN dotnet publish "src/BlazorEnterpriseStarter.App/BlazorEnterpriseStarter.App.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BlazorEnterpriseStarter.App.dll"]
