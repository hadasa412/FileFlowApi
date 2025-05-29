# שלב בסיס להרצת האפליקציה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# שלב בנייה ופרסום
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src

COPY ["data/data.csproj", "data/"]
COPY ["service/service.csproj", "service/"]
COPY ["core/core.csproj", "core/"]
COPY ["FileFlowApi/FILEFLOWAPI.csproj", "FileFlowApi/"]

RUN dotnet restore "FileFlowApi/FILEFLOWAPI.csproj"
COPY . .
WORKDIR "/src/FileFlowApi"
RUN dotnet publish "FILEFLOWAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# שלב סופי – הרצה
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FILEFLOWAPI.dll"]
