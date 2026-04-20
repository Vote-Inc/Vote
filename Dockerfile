FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Vote.Domain/Vote.Domain.csproj Vote.Domain/
COPY Vote.Application/Vote.Application.csproj Vote.Application/
COPY Vote.Infrastructure/Vote.Infrastructure.csproj Vote.Infrastructure/
COPY Vote.API/Vote.API.csproj Vote.API/

RUN dotnet restore Vote.API/Vote.API.csproj

COPY . .
RUN dotnet publish Vote.API/Vote.API.csproj \
-c Release \
-o /app/publish \
--no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system --gid 1001 appgroup && \
    useradd --system --uid 1001 --gid appgroup appuser

COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "Vote.API.dll"]
