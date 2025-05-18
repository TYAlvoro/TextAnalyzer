FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileStoreService/FileStoreService.csproj", "FileStoreService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "FileStoreService/FileStoreService.csproj"

COPY . .
WORKDIR "/src/FileStoreService"
RUN dotnet publish "FileStoreService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileStoreService.dll"]
