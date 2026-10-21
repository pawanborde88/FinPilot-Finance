# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY ["FinPilot.sln", "./"]
COPY ["src/FinPilot.Api/FinPilot.Api.csproj", "src/FinPilot.Api/"]
COPY ["src/FinPilot.Application/FinPilot.Application.csproj", "src/FinPilot.Application/"]
COPY ["src/FinPilot.Domain/FinPilot.Domain.csproj", "src/FinPilot.Domain/"]
COPY ["src/FinPilot.Infrastructure/FinPilot.Infrastructure.csproj", "src/FinPilot.Infrastructure/"]
COPY ["tests/FinPilot.UnitTests/FinPilot.UnitTests.csproj", "tests/FinPilot.UnitTests/"]

RUN dotnet restore "FinPilot.sln"

# Copy all source files and build
COPY . .
WORKDIR "/src/src/FinPilot.Api"
RUN dotnet publish "FinPilot.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinPilot.Api.dll"]
