FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FileAnalysisService/FileAnalysisService.csproj", "FileAnalysisService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "FileAnalysisService/FileAnalysisService.csproj"

COPY . .
WORKDIR "/src/FileAnalysisService"
RUN dotnet publish "FileAnalysisService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileAnalysisService.dll"]
