FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["DragonBallLibrary.ApiService/DragonBallLibrary.ApiService.csproj", "DragonBallLibrary.ApiService/"]
COPY ["DragonBallLibrary.ServiceDefaults/DragonBallLibrary.ServiceDefaults.csproj", "DragonBallLibrary.ServiceDefaults/"]

RUN dotnet restore "DragonBallLibrary.ApiService/DragonBallLibrary.ApiService.csproj"

# Copy source code
COPY . .

WORKDIR "/src/DragonBallLibrary.ApiService"
RUN dotnet build "DragonBallLibrary.ApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DragonBallLibrary.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DragonBallLibrary.ApiService.dll"]